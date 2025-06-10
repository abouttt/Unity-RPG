using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private SerializedProperty _itemIdProp;
    private ItemDatabase _itemDatabase;

    private void OnEnable()
    {
        _itemIdProp = serializedObject.FindProperty($"<{nameof(ItemData.Id)}>k__BackingField");
        _itemDatabase = Resources.Load<ItemDatabase>("Databases/ItemDatabase");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();
        if (EditorGUI.EndChangeCheck())
        {
            // 아이템 아이디가 파스칼형식이면 강제로 대문자와 스네이크 형식으로 변환
            string originalItemId = _itemIdProp.stringValue;
            string newItemId = Utility.ToSnake(originalItemId).ToUpper();
            if (originalItemId != newItemId)
            {
                _itemIdProp.stringValue = newItemId;
            }
        }

        var itemData = _itemDatabase.FindItemById(_itemIdProp.stringValue);
        if (itemData != null)
        {
            EditorGUILayout.HelpBox(
                $"Id already exist : {AssetDatabase.GetAssetPath(itemData)}",
                MessageType.Error
            );
        }

        serializedObject.ApplyModifiedProperties();
    }
}
