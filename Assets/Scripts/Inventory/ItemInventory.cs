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

    public int AddItem(ItemData itemData, int quantity = 1)
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
                    var sameItem = GetItem<StackableItem>(sameItemIndex);
                    quantity = sameItem.StackAndGetExcess(quantity);
                    index = sameItemIndex;

                    if (quantity <= 0)
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
                SetItem(itemData, emptyIndex, quantity);
                quantity = isStackable ? Mathf.Max(0, quantity - stackableData.MaxQuantity) : quantity - 1;
                index = emptyIndex;

                if (quantity <= 0)
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

    public void RemoveItem(int index)
    {
        if (!HasItem(index))
        {
            return;
        }

        _items[index].Destroy();
        _items[index] = null;
        _count--;
        InventoryChanged?.Invoke(null, index);
    }

    public void SetItem(ItemData itemData, int index, int quantity = 1)
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

        RemoveItem(index);

        var newItem = ItemFactory.CreateItem(itemData, quantity);
        _items[index] = newItem;
        _count++;
        InventoryChanged?.Invoke(newItem, index);
    }

    public void MoveItem(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!TryMergeItem(fromIndex, toIndex))
        {
            SwapItem(fromIndex, toIndex);
        }
    }

    public void SplitItem(int fromIndex, int toIndex, int quantity)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (quantity <= 0)
        {
            return;
        }

        if (!HasItem(fromIndex) || HasItem(toIndex))
        {
            return;
        }

        var fromItem = GetItem<StackableItem>(fromIndex);
        if (fromItem == null)
        {
            return;
        }

        int remaining = fromItem.Quantity - quantity;
        if (remaining <= 0)
        {
            SwapItem(fromIndex, toIndex);
        }
        else
        {
            fromItem.Quantity = remaining;
            SetItem(fromItem.StackableData, toIndex, quantity);
        }
    }

    public bool ConsumeItem(int index)
    {
        if (!HasItem(index))
        {
            return false;
        }

        var comsumableItem = GetItem<ConsumableItem>(index);
        if (comsumableItem == null)
        {
            return false;
        }

        bool succeeded = comsumableItem.Consume();

        if (succeeded)
        {
            if (comsumableItem.IsEmpty)
            {
                RemoveItem(index);
            }
        }

        return succeeded;
    }

    public T GetItem<T>(int index) where T : Item
    {
        return IsValidIndex(index) ? _items[index] as T : null;
    }

    public int IndexOf(Item item)
    {
        return item != null ? _items.IndexOf(item) : -1;
    }

    public bool HasItem(int index)
    {
        return IsValidIndex(index) && _items[index] != null;
    }

    public bool IsValidIndex(int index)
    {
        return index >= 0 && index < _capacity;
    }

    private bool TryMergeItem(int fromIndex, int toIndex)
    {
        var fromItem = GetItem<StackableItem>(fromIndex);
        var toItem = GetItem<StackableItem>(toIndex);

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
            RemoveItem(fromIndex);
        }

        return true;
    }

    private void SwapItem(int fromIndex, int toIndex)
    {
        (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);
        InventoryChanged?.Invoke(_items[fromIndex], fromIndex);
        InventoryChanged?.Invoke(_items[toIndex], toIndex);
    }
}
