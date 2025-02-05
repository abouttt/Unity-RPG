using System;
using UnityEngine;

public class Item : IItem
{
    public event Action<Item> Destroyed;

    public ItemData Data { get; private set; }

    public Item(ItemData data)
    {
        Data = data;
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
        Destroyed = null;
    }
}
