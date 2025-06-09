using System;
using UnityEngine;

public interface IStackable
{
    event Action<IStackable> StackChanged;

    int Quantity { get; }
    int MaxQuantity { get; }
}
