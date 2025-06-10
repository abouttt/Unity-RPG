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
            // ������ ���̵� �Ľ�Į�����̸� ������ �빮�ڿ� ������ũ �������� ��ȯ
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
