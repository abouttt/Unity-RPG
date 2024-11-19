using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemData), true)]
public class ItemDataEditor : Editor
{
    private SerializedProperty _itemIdProp;

    private void OnEnable()
    {
        _itemIdProp = serializedObject.FindProperty($"<{nameof(ItemData.ItemId)}>k__BackingField");
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();

        // 아이템 아이디가 파스칼형식이면 강제로 대문자와 스네이크 형식으로 변환
        string originalItemId = _itemIdProp.stringValue;
        string newItemId = originalItemId.ToSnake().ToUpper();
        if (!originalItemId.Equals(newItemId))
        {
            //_itemIdProp.stringValue = newItemId;
            //var itemData = ItemDatabase.Instance.FindItemById(newItemId);
            //if (itemData != null)
            //{
            //    Debug.LogWarning($"{newItemId} id already exist : {AssetDatabase.GetAssetPath(itemData)}");
            //}
        }

        serializedObject.ApplyModifiedProperties();
    }
}
