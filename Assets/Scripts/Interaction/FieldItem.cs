using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class FieldItem : Interactable
{
    public IReadOnlyDictionary<ItemData, int> Items => _items;

    [SerializeField]
    private bool _destroyWhenEmpty = true;

    [SerializeField, SerializedDictionary("Item", "Quantity")]
    private SerializedDictionary<ItemData, int> _items;

    private void Start()
    {
        if ((_items == null || _items.Count == 0) && _destroyWhenEmpty)
        {
            ResourceManager.Destroy(gameObject);
        }
    }

    protected override void OnInteract()
    {
        base.OnInteract();
        UIManager.Show<UI_LootPopup>().SetFieldItem(this);
    }

    public void AddItem(ItemData itemData, int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        if (!_items.ContainsKey(itemData))
        {
            _items.Add(itemData, 0);
        }

        _items[itemData] += quantity;
    }

    public void RemoveItem(ItemData itemData, int quantity)
    {
        if (quantity <= 0)
        {
            return;
        }

        if (_items.ContainsKey(itemData))
        {
            _items[itemData] -= quantity;

            if (_items[itemData] <= 0)
            {
                _items.Remove(itemData);
            }
        }

        if (_items.Count == 0 && _destroyWhenEmpty)
        {
            ResourceManager.Destroy(gameObject);
        }
    }
}
