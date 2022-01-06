using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;

[CustomEditor(typeof(Star)), CanEditMultipleObjects]
public class StarEditor : Editor
{
    private SerializedProperty _center;
    private SerializedProperty _points;
    private SerializedProperty _frequency;
    private Star _star;
    private void OnEnable()
    {
        _star = target as Star;
        _center = serializedObject.FindProperty("_center");
        _points = serializedObject.FindProperty("_points");
        _frequency = serializedObject.FindProperty("_frequency");
    }
    
    private void OnSceneGUI()
    {
        if (!(target is Star star))
        {
            return;
        }
        var starTransform = star.transform;
        var angle = -360f / (star.frequency * star.points.Length);
        for (var i = 0; i < star.points.Length; i++)
        {
            var rotation = Quaternion.Euler(0f, 0f, angle * i);
            var oldPoint = starTransform.TransformPoint(rotation *
                                                        star.points[i].Position);
            var newPoint = Handles.FreeMoveHandle(oldPoint, Quaternion.identity,
                0.02f, Vector3.one, Handles.DotHandleCap);
            if (oldPoint == newPoint)
            {
                continue;
            }
            star.points[i].Position = Quaternion.Inverse(rotation) *
                                      starTransform.InverseTransformPoint(newPoint);
            star.UpdateMesh();
        }
    }

    
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(_center);
        int i = 0;
        foreach (var point in _points)
        {
            EditorGUILayout.PropertyField(point as SerializedProperty);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Up", GUILayout.Width(80)))
                _star.MovePointUp(i);
            if (GUILayout.Button("Down", GUILayout.Width(80)))
                _star.MovePointDown(i);
            if (GUILayout.Button("+", GUILayout.Width(40)))
                _star.AddNewPoint(i);
            if (GUILayout.Button("-", GUILayout.Width(40)))
                _star.DeletePoint(i);
            EditorGUILayout.EndHorizontal();
            i++;
        }
        EditorGUILayout.IntSlider(_frequency, 1, 20);
        var totalPoints = _frequency.intValue * _points.arraySize;
        if (totalPoints < 3)
        {
            EditorGUILayout.HelpBox("At least three points are needed.",
                MessageType.Warning);
        }
        else
        {
            EditorGUILayout.HelpBox(totalPoints + " points in total.",
                MessageType.Info);
        }
        if (!serializedObject.ApplyModifiedProperties() &&
            (Event.current.type != EventType.ExecuteCommand ||
             Event.current.commandName != "UndoRedoPerformed"))
        {
            return;
        }
        foreach (var obj in targets)
        {
            if (obj is Star star)
            {
                star.UpdateMesh();
            }
        }

    }
}