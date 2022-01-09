using System;
using System.Collections.Generic;
using System.Linq;
using Main;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(Sprite))]
public class GenerationWindow: EditorWindow
{
    private static Vector2 _windowMinSize = Vector2.one * 1000f;
    private Rect _listRect = new Rect(Vector2.zero, _windowMinSize);
    private Material _material;
    private MapData _mapData;
    private SerializedObject _dataObjects;
    private ReorderableList _reorderableList;
    private static EditorWindow _editorWindow;
    private ListView _listView;
    [MenuItem("Window/Generate Map")]
    public static void ShowWindow()
    {
        _editorWindow = EditorWindow.GetWindow(typeof(GenerationWindow));
    }

    public void OnEnable()
    {
        _material = new Material(Shader.Find("Hidden/Internal-Colored"));
        LoadData();
    }

    public void CreateGUI()
    { 
        rootVisualElement.Add(new Image());
        _listView = new ListView();
        rootVisualElement.Add(_listView);
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private int _heigth;
    private Vector2 _scrollPosition;
    private void OnGUI()
    {
        //_scrollPosition = GUILayout.BeginScrollView(_scrollPosition,false,true, GUILayout.ExpandWidth(true));
        GUILayout.BeginHorizontal(EditorStyles.helpBox);
        _heigth = ReDrawGraphic();
        GUILayout.EndHorizontal();
        
        // EditorGUILayout.BeginHorizontal();
        if (_dataObjects == null)
        {
            EditorGUI.HelpBox(new Rect(0f,10f,400f,100f),
                "No Scriptable Object!",MessageType.Warning);
            return;
        }
        else if(_dataObjects!= null)
        {
          
            _dataObjects.Update();
            _listRect.y = 400f;//_heigth+30;
            _reorderableList.DoList(_listRect);
            _dataObjects.ApplyModifiedProperties();
        }
        // EditorGUILayout.EndHorizontal();
        //GUILayout.EndScrollView();
        
    }

    private int ReDrawGraphic()
    {
        var height =  50+30*_reorderableList.count;
        Rect layoutRectangle = GUILayoutUtility.GetRect(100,10000,200,380f);
        if(Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(layoutRectangle);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            _material.SetPass(0);
            GL.Begin(GL.LINES);
            DrawPlanet(layoutRectangle,Color.yellow,new PlanetStruct()
            {
                PlanetOrbitData = null,Position = new Vector3(0,0,0),Speed = 0,Radius = 100
            });
            foreach (var planetStruct in _mapData.PlanetStructs)
            {
                DrawPlanet(layoutRectangle,Color.blue,planetStruct);
            }
            GL.End();
            GL.PopMatrix();
            GUI.EndClip();
        }
        return height;
    }
    
    private void DrawPlanet(Rect layoutRectangle,Color color,PlanetStruct planetStruct)
    {
        const float DEG2RAD = 3.14159f/180;
        GL.Color(color);
        float x = planetStruct.Position.x+layoutRectangle.width / 2;//layoutRectangle.width / 2;
        float y = planetStruct.Position.y+layoutRectangle.height / 2;
        for (int i=0; i< 360; i++)
        { 
            float degInRad = i*DEG2RAD;
            GL.Vertex(new Vector3(x, y, 0));
            GL.Vertex(new Vector3(x + Mathf.Cos(degInRad)*planetStruct.Radius/4,
                y + Mathf.Sin(degInRad)*planetStruct.Radius/4, 0));
        }
    }
    // public void AddNewPoint(int index)
    // {
    //     _planetStructs = ReSorting(index,_planetStructs.ToList());
    // }
    //
    // private PlanetStruct[] ReSorting(int index, List<PlanetStruct> copyArray)
    // {
    //         PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
    //         copyArray.CopyTo(additional);
    //         copyArray.Add(new PlanetStruct());
    //         for (int i = index; i < copyArray.Count - 1; i++)
    //         {
    //             copyArray[i + 1] = additional[i];
    //         }
    //         copyArray[index+1] = new PlanetStruct();
    //         return copyArray.ToArray();
    // }
    //
    // public void DeletePoint(int index)
    // {
    //         var copyArray = _planetStructs.ToList();
    //         copyArray.RemoveAt(index);
    //         _planetStructs = copyArray.ToArray();
    // }
    //
    // public void MovePointDown(int index) 
    // {
    //         if (index != _planetStructs.Length - 1)
    //         {
    //             var copyArray = _planetStructs.ToList();
    //             PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
    //             copyArray.CopyTo(additional);
    //             copyArray[index + 1] = additional[index];
    //             copyArray[index] = additional[index + 1];
    //             _planetStructs = copyArray.ToArray();
    //         }
    // }
    //
    // public void MovePointUp(int index)
    // {
    //         if (index!=0)
    //         {
    //             var copyArray = _planetStructs.ToList();
    //             PlanetStruct[] additional = new PlanetStruct[copyArray.Count];
    //             copyArray.CopyTo(additional);
    //             copyArray[index - 1] = additional[index];
    //             copyArray[index] = additional[index - 1];
    //             _planetStructs = copyArray.ToArray();
    //         }
    // }
    //
    // public void SetBasePlanet() 
    // {
    //         if (_planetStructs==null)
    //             _planetStructs = new PlanetStruct[]{new PlanetStruct(){PlanetOrbit = null, Position = Vector3.zero,Speed = 0.0f}};
    // }

    private void LoadData()
    {
        _mapData = Resources.Load<MapData>("MapData");
        if (_mapData)
        {
            _dataObjects = new SerializedObject(_mapData);
            _reorderableList = new ReorderableList(_dataObjects, _dataObjects.FindProperty("PlanetStructs"), 
                true, true, true, true);
            _reorderableList.drawHeaderCallback = (rect) => EditorGUI.LabelField(rect, "Planets");
            _reorderableList.elementHeightCallback = (int index) =>
            {
                SerializedProperty namesProp = _reorderableList.serializedProperty.GetArrayElementAtIndex(index);
                return EditorGUI.GetPropertyHeight(namesProp) + EditorGUIUtility.standardVerticalSpacing;
            };
            _reorderableList.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                rect.y += 2f;
                rect.height = EditorGUIUtility.singleLineHeight;
                GUIContent planetLabel = new GUIContent($"Planet {index}");
                EditorGUI.PropertyField(rect,
                    _reorderableList.serializedProperty.GetArrayElementAtIndex(index),
                    planetLabel,true);
            };
        }
    }
}
