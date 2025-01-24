using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UISettings), true)]
public class UISettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        DrawDefaultInspector();

        var settings = (UISettings)target;

        if (GUILayout.Button("Refresh UI Prefabs"))
        {
            settings.RefreshUIPrefabs();
        }
    }
}
