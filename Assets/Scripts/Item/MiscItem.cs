using UnityEngine;

public class MiscItem : StackableItem
{
    public MiscItemData MiscData => Data as MiscItemData;

    public MiscItem(MiscItemData data, int quantity)
        : base(data, quantity)
    {
    }
}
