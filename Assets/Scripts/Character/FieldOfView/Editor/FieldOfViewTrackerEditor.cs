using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(FieldOfViewTracker))]
public class FieldOfViewTrackerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
        GUI.enabled = true;

        EditorGUILayout.PropertyField(serializedObject.FindProperty("_distance"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("_obstacle"));

        SerializedProperty horizontalAngleProp = serializedObject.FindProperty("_horizontal");
        EditorGUILayout.PropertyField(horizontalAngleProp);
        if (horizontalAngleProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_horizontalClamp"));
            EditorGUI.indentLevel--;
        }

        SerializedProperty verticalAngleProp = serializedObject.FindProperty("_vertical");
        EditorGUILayout.PropertyField(verticalAngleProp);
        if (verticalAngleProp.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(serializedObject.FindProperty("_verticalClamp"));
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
