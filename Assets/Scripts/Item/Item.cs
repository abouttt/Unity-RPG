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

    public void Destroy()
    {
        if (Data.IsDestructible)
        {
            Destroyed?.Invoke(this);
            Destroyed = null;
        }
    }
}
