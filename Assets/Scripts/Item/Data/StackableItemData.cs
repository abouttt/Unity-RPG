using UnityEngine;

public abstract class StackableItemData : ItemData
{
    [field: Header("Stackable Data")]
    [field: SerializeField]
    public int MaxQuantity { get; private set; } = 99;

    public abstract StackableItem CreateItem(int quantity);
}
