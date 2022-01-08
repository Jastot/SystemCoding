using Main;
using Mechanics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

[CustomEditor(typeof(SolarSystemCreator)), CanEditMultipleObjects]
public class SolarSystemEditor:Editor
{ 
    private SerializedProperty _planetStructs;
    private SolarSystemCreator _solarSystemNetworkManager;
    private void OnEnable()
    {
        _solarSystemNetworkManager = target as SolarSystemCreator;
        _solarSystemNetworkManager.SetBasePlanet();
        _planetStructs = serializedObject.FindProperty("_planetStructs");
    }
    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        int i = 0;
        foreach (var point in _planetStructs)
        {
            EditorGUILayout.PropertyField(point as SerializedProperty);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up", GUILayout.Width(80)))
                _solarSystemNetworkManager.MovePointUp(i);
            if (GUILayout.Button("Down", GUILayout.Width(80)))
                _solarSystemNetworkManager.MovePointDown(i);
            if (GUILayout.Button("+", GUILayout.Width(40)))
                _solarSystemNetworkManager.AddNewPoint(i);
            if (GUILayout.Button("-", GUILayout.Width(40)))
                _solarSystemNetworkManager.DeletePoint(i);
            EditorGUILayout.EndHorizontal();
            i++;
        }
        if (!serializedObject.ApplyModifiedProperties() &&
            (Event.current.type != EventType.ExecuteCommand ||
             Event.current.commandName != "UndoRedoPerformed"))
        {
            return;
        }
        // EditorGUILayout.IntSlider(_frequency, 1, 20);
        // var totalPoints = _frequency.intValue * _points.arraySize;
        // if (totalPoints < 3)
        // {
        //     EditorGUILayout.HelpBox("At least three points are needed.",
        //         MessageType.Warning);
        // }
        // else
        // {
        //     EditorGUILayout.HelpBox(totalPoints + " points in total.",
        //         MessageType.Info);
        // }
        // if (!serializedObject.ApplyModifiedProperties() &&
        //     (Event.current.type != EventType.ExecuteCommand ||
        //      Event.current.commandName != "UndoRedoPerformed"))
        // {
        //     return;
        // }
        // foreach (var obj in targets)
        // {
        //     if (obj is Star star)
        //     {
        //         star.UpdateMesh();
        //     }
        // }
    }
}
