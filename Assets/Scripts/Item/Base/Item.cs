using System;
using UnityEngine;

public class Item : IItem
{
    public event Action<Item> Destroyed;

    public ItemData Data { get; private set; }

    public Item(ItemData itemData)
    {
        Data = itemData;
    }

    public void Destroy()
    {
        Destroyed?.Invoke(this);
        Destroyed = null;
    }
}
