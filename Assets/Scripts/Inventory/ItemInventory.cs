using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public event Action<Item, int> InventoryChanged;

    public int Capacity => _capacity;
    public int Count => _count;

    [SerializeField]
    private int _capacity;

    [SerializeField, ReadOnly]
    private int _count;

    private List<Item> _items;

    private void Awake()
    {
        _items = new(_capacity);
        for (int i = 0; i < _capacity; i++)
        {
            _items.Add(null);
        }
    }

    public int Add(ItemData itemData, int quantity = 1)
    {
        if (itemData == null)
        {
            return -1;
        }

        if (quantity <= 0)
        {
            return -1;
        }

        // 같은 아이템에 개수 더하기 시도
        if (itemData is StackableItemData)
        {
            for (int i = 0; i < _capacity; i++)
            {
                if (_items[i] is StackableItem otherItem && otherItem.Data.Equals(itemData))
                {
                    quantity = otherItem.StackAndGetExcess(quantity);
                    if (quantity <= 0)
                    {
                        break;
                    }
                }
            }
        }

        // 빈 공간에 아이템 추가 시도
        for (int i = 0; i < _capacity; i++)
        {
            if (_count == _capacity)
            {
                break;
            }

            if (_items[i] != null)
            {
                continue;
            }

            Set(itemData, i, quantity);
            quantity = itemData is StackableItemData stackableData
                     ? Mathf.Max(0, quantity - stackableData.MaxQuantity)
                     : quantity - 1;

            if (quantity <= 0)
            {
                break;
            }
        }

        return quantity;
    }

    public bool Remove(int index, bool coercion = false)
    {
        if (!Has(index))
        {
            return false;
        }

        if (!_items[index].Destroy(coercion))
        {
            return false;
        }

        _items[index] = null;
        _count--;
        InventoryChanged?.Invoke(null, index);

        return true;
    }

    public bool Set(ItemData itemData, int index, int quantity = 1)
    {
        if (itemData == null)
        {
            return false;
        }

        if (!IsValidIndex(index))
        {
            return false;
        }

        if (quantity < 1)
        {
            return false;
        }

        if (!Remove(index))
        {
            return false;
        }

        var newItem = itemData is StackableItemData stackableData
                    ? stackableData.CreateItem(quantity)
                    : itemData.CreateItem();

        _items[index] = newItem;
        _count++;
        InventoryChanged?.Invoke(newItem, index);

        return true;
    }

    public void Move(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!TryMerge(fromIndex, toIndex))
        {
            Swap(fromIndex, toIndex);
        }
    }

    public bool Split(int fromIndex, int toIndex, int quantity)
    {
        if (fromIndex == toIndex)
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (!Has(fromIndex) || Has(toIndex))
        {
            return false;
        }

        if (_items[fromIndex] is not StackableItem fromItem)
        {
            return false;
        }

        int remaining = fromItem.Quantity - quantity;
        if (remaining <= 0)
        {
            Swap(fromIndex, toIndex);
        }
        else
        {
            fromItem.Quantity = remaining;
            Set(fromItem.StackableData, toIndex, quantity);
        }

        return true;
    }

    public bool Consume(int index)
    {
        if (!Has(index))
        {
            return false;
        }

        if (_items[index] is not ConsumableItem consumableItem)
        {
            return false;
        }

        bool succeeded = consumableItem.Consume(gameObject);
        if (succeeded && consumableItem.IsEmpty)
        {
            Remove(index);
        }

        return succeeded;
    }

    public T Get<T>(int index) where T : Item
    {
        return IsValidIndex(index) ? _items[index] as T : null;
    }

    public bool Has(int index)
    {
        return IsValidIndex(index) && _items[index] != null;
    }

    public int IndexOf(Item item)
    {
        return item != null ? _items.IndexOf(item) : -1;
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < _capacity;
    }

    private bool TryMerge(int fromIndex, int toIndex)
    {
        var fromItem = Get<StackableItem>(fromIndex);
        var toItem = Get<StackableItem>(toIndex);

        if (fromItem == null || toItem == null)
        {
            return false;
        }

        if (!fromItem.Data.Equals(toItem.Data))
        {
            return false;
        }

        if (toItem.IsMax)
        {
            return false;
        }

        fromItem.Quantity = toItem.StackAndGetExcess(fromItem.Quantity);
        if (fromItem.IsEmpty)
        {
            Remove(fromIndex);
        }

        return true;
    }

    private void Swap(int fromIndex, int toIndex)
    {
        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
        {
            return;
        }

        (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);
        InventoryChanged?.Invoke(_items[fromIndex], fromIndex);
        InventoryChanged?.Invoke(_items[toIndex], toIndex);
    }
}
