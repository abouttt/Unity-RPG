using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(DataBinding), true)]
public class DataBindingEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        var dataBinding = (DataBinding)target;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.PropertyField(serializedObject.FindProperty("Target"));
        EditorGUI.EndDisabledGroup();

        if (dataBinding.AutoRefreshDataID)
        {
            if (dataBinding.DataID != dataBinding.gameObject.name)
            {
                dataBinding.DataID = dataBinding.gameObject.name;
            }

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DataID"));
            EditorGUI.EndDisabledGroup();
        }
        else
        {
            EditorGUILayout.PropertyField(serializedObject.FindProperty("DataID"));
        }

        EditorGUILayout.PropertyField(serializedObject.FindProperty("AutoRefreshDataID"));

        serializedObject.ApplyModifiedProperties();
    }
}
