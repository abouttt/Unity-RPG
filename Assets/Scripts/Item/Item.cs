using System;
using UnityEngine;

public abstract class Item : IItem
{
    public event Action<Item> Destroyed;

    public ItemData Data { get; private set; }

    public Item(ItemData data)
    {
        Data = data;
    }

    public bool Destroy(bool coercion = false)
    {
        if (!Data.IsDestructible && !coercion)
        {
            return false;
        }

        Destroyed?.Invoke(this);
        Destroyed = null;

        return true;
    }
}
