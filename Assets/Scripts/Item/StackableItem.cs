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
            int clampedQuantity = Mathf.Clamp(value, 0, MaxQuantity);
            if (_quantity != clampedQuantity)
            {
                _quantity = clampedQuantity;
                StackChanged?.Invoke(this);
            }
        }
    }

    public int MaxQuantity => StackableData.MaxQuantity;
    public bool IsMax => _quantity >= MaxQuantity;
    public bool IsEmpty => _quantity <= 0;

    private int _quantity;

    public StackableItem(StackableItemData data, int quantity)
        : base(data)
    {
        Quantity = quantity;
    }

    public int StackAndGetExcess(int quantity)
    {
        int nextQuantity = _quantity + quantity;
        Quantity = nextQuantity;
        return nextQuantity > MaxQuantity ? nextQuantity - MaxQuantity : 0;
    }
}
