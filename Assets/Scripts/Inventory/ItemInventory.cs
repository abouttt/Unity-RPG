using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemInventory : MonoBehaviour
{
    public event Action<Item, int> Changed;

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

        var stackableData = itemData as StackableItemData;

        // 같은 아이템에 개수 더하기 시도
        if (stackableData != null)
        {
            foreach (var item in _items)
            {
                if (item == null)
                {
                    continue;
                }

                if (item.Data.Equals(itemData))
                {
                    quantity = (item as StackableItem).StackAndGetExcess(quantity);
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

            Item newItem;

            if (stackableData != null)
            {
                newItem = stackableData.CreateItem(quantity);
                quantity = Mathf.Max(0, quantity - stackableData.MaxQuantity);
            }
            else
            {
                newItem = itemData.CreateItem();
                quantity--;
            }

            _items[i] = newItem;
            _count++;
            Changed?.Invoke(newItem, i);

            if (quantity <= 0)
            {
                break;
            }
        }

        return quantity;
    }

    public bool Remove(int index, bool force = false)
    {
        if (!Has(index))
        {
            return false;
        }

        if (!_items[index].Destroy(force))
        {
            return false;
        }

        _items[index] = null;
        _count--;
        Changed?.Invoke(null, index);

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

        if (quantity <= 0)
        {
            return false;
        }

        if (_items[index] != null)
        {
            if (!_items[index].Destroy())
            {
                return false;
            }
            else
            {
                _count--;
            }
        }

        var newItem = itemData is StackableItemData stackableData
                    ? stackableData.CreateItem(quantity)
                    : itemData.CreateItem();

        _items[index] = newItem;
        _count++;
        Changed?.Invoke(newItem, index);

        return true;
    }

    public void Move(int fromIndex, int toIndex)
    {
        if (fromIndex == toIndex)
        {
            return;
        }

        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
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

        if (!IsValidIndex(fromIndex) || !IsValidIndex(toIndex))
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (_items[fromIndex] == null || _items[toIndex] != null)
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
            var newItem = fromItem.StackableData.CreateItem(quantity);
            _items[toIndex] = newItem;
            _count++;
            Changed?.Invoke(newItem, toIndex);
        }

        return true;
    }

    public bool SubtractQuantity(int index, int quantity)
    {
        if (!Has(index))
        {
            return false;
        }

        if (quantity <= 0)
        {
            return false;
        }

        if (_items[index] is not StackableItem stackableItem)
        {
            return false;
        }

        stackableItem.Quantity -= quantity;
        if (stackableItem.IsEmpty)
        {
            DestroyByForce(index);
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
            DestroyByForce(index);
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
        if (_items[fromIndex] is not StackableItem fromItem || _items[toIndex] is not StackableItem toItem)
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
            DestroyByForce(fromIndex);
        }

        return true;
    }

    private void Swap(int fromIndex, int toIndex)
    {
        (_items[fromIndex], _items[toIndex]) = (_items[toIndex], _items[fromIndex]);
        Changed?.Invoke(_items[fromIndex], fromIndex);
        Changed?.Invoke(_items[toIndex], toIndex);
    }

    private void DestroyByForce(int index)
    {
        _items[index].Destroy(true);
        _items[index] = null;
        _count--;
        Changed?.Invoke(null, index);
    }
}
