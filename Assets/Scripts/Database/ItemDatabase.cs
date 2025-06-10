using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Database/Item Database", fileName = "ItemDatabase")]
public class ItemDatabase : ScriptableObject
{
    public IReadOnlyCollection<ItemData> Items => _items;

    [SerializeField]
    private List<ItemData> _items;

    public ItemData FindItemById(string id)
    {
        return _items.FirstOrDefault(item => item != null && item.Id == id);
    }

#if UNITY_EDITOR
    [ContextMenu("Find Items")]
    private void FindItems()
    {
        _items = new();

        foreach (var guid in AssetDatabase.FindAssets($"t:{typeof(ItemData)}"))
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            var itemData = AssetDatabase.LoadAssetAtPath<ItemData>(assetPath);
            _items.Add(itemData);
        }

        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssets();
    }

    [ContextMenu("Remove Null Items")]
    private void RemoveNullItems()
    {
        _items.RemoveAll(item => item == null);
    }
#endif
}
