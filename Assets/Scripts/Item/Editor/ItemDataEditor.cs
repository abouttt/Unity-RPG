using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private ItemDatabase _itemDatabase;
    private SerializedProperty _itemIdProp;

    private void OnEnable()
    {
        _itemDatabase = Resources.Load<ItemDatabase>("Databases/ItemDatabase");
        _itemIdProp = serializedObject.FindProperty($"<{nameof(ItemData.Id)}>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        if (GUI.changed)
        {
            // 아이템 아이디가 파스칼형식이면 강제로 대문자와 스네이크 형식으로 변환
            string originalItemId = _itemIdProp.stringValue;
            string newItemId = originalItemId.ToSnake().ToUpper();
            if (!originalItemId.Equals(newItemId))
            {
                _itemIdProp.stringValue = newItemId;
                var itemData = _itemDatabase.FindItemById(newItemId);
                if (itemData != null)
                {
                    Debug.LogWarning($"{newItemId} id already exist : {AssetDatabase.GetAssetPath(itemData)}");
                }
            }
        }

        serializedObject.ApplyModifiedProperties();
    }
}
