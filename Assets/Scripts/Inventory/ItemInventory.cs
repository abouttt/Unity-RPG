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
        _items = new(new Item[_capacity]);
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

        var stackableData = itemData as StackableItemData;
        bool isStackable = stackableData != null;

        // 같은 아이템에 개수 더하기 시도
        if (isStackable)
        {
            for (int index = 0; index < _capacity; index++)
            {
                int sameItemIndex = _items.FindIndex(index, item => item is StackableItem && item.Data.Equals(itemData));
                if (sameItemIndex != -1)
                {
                    var sameItem = Get<StackableItem>(sameItemIndex);
                    quantity = sameItem.StackAndGetExcess(quantity);
                    if (quantity > 0)
                    {
                        index = sameItemIndex;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    break;
                }
            }
        }

        // 빈 공간에 아이템 추가 시도
        for (int index = 0; index < _capacity; index++)
        {
            int emptyIndex = _capacity == _count ? -1 : _items.FindIndex(index, item => item == null);
            if (emptyIndex != -1)
            {
                Set(itemData, emptyIndex, quantity);
                quantity = isStackable ? Mathf.Max(0, quantity - stackableData.MaxQuantity) : quantity - 1;
                if (quantity > 0)
                {
                    index = emptyIndex;
                }
                else
                {
                    break;
                }
            }
            else
            {
                break;
            }
        }

        return quantity;
    }

    public void Remove(int index)
    {
        if (!Has(index))
        {
            return;
        }

        _items[index].Destroy();
        _items[index] = null;
        _count--;
        InventoryChanged?.Invoke(null, index);
    }

    public void Set(ItemData itemData, int index, int quantity = 1)
    {
        if (itemData == null)
        {
            return;
        }

        if (!IsValidIndex(index))
        {
            return;
        }

        if (quantity <= 0)
        {
            return;
        }

        Remove(index);

        var newItem = itemData is StackableItemData stackableData
                    ? stackableData.CreateItem(quantity)
                    : itemData.CreateItem();

        _items[index] = newItem;
        _count++;
        InventoryChanged?.Invoke(newItem, index);
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

    public void Split(int fromIndex, int toIndex, int quantity)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (quantity <= 0)
        {
            return;
        }

        if (!Has(fromIndex) || Has(toIndex))
        {
            return;
        }

        var fromItem = Get<StackableItem>(fromIndex);
        if (fromItem == null)
        {
            return;
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
    }

    public bool Consume(int index)
    {
        if (!Has(index))
        {
            return false;
        }

        var consumableItem = Get<ConsumableItem>(index);
        if (consumableItem == null)
        {
            return false;
        }

        bool succeeded = consumableItem.Consume(gameObject);
        if (succeeded)
        {
            if (consumableItem.IsEmpty)
            {
                Remove(index);
            }
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
