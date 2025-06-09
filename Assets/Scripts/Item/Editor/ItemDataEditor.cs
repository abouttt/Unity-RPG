using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private SerializedProperty _itemIdProp;

    private void OnEnable()
    {
        _itemIdProp = serializedObject.FindProperty($"<{nameof(ItemData.Id)}>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        serializedObject.Update();

        if (GUI.changed)
        {
            string originalItemId = _itemIdProp.stringValue;
            string newItemId = Utility.ToSnake(originalItemId).ToUpper();
            if (originalItemId != newItemId)
            {
                _itemIdProp.stringValue = newItemId;
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
