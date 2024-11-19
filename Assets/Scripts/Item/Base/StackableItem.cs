using System;
using UnityEngine;

public abstract class StackableItem : Item, IStackable
{
    public event Action<IStackable> StackChanged;

    public StackableItemData StackableData => Data as StackableItemData;

    public int Quantity
    {
        get => _quantity;
        set
        {
            int prevQuantity = _quantity;
            _quantity = Mathf.Clamp(value, 0, MaxQuantity);
            if (_quantity != prevQuantity)
            {
                StackChanged?.Invoke(this);
            }
        }
    }

    public int MaxQuantity => StackableData.MaxQuantity;
    public bool IsMax => _quantity >= MaxQuantity;
    public bool IsEmpty => _quantity <= 0;

    private int _quantity;

    public StackableItem(StackableItemData stackableData, int quantity)
        : base(stackableData)
    {
        Quantity = quantity;
    }

    public int StackAndGetExcess(int quantity)
    {
        int nextQuantity = _quantity + quantity;
        Quantity += quantity;
        return nextQuantity > MaxQuantity ? nextQuantity - MaxQuantity : 0;
    }
}
