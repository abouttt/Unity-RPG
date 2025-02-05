using UnityEngine;

public class MiscellaneousItem : StackableItem
{
    public MiscellaneousItemData MiscellaneousData => Data as MiscellaneousItemData;

    public MiscellaneousItem(MiscellaneousItemData data, int quantity)
        : base(data, quantity)
    { }
}
