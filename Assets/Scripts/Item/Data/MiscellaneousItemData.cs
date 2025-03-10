using UnityEngine;

[CreateAssetMenu(menuName = "Item/Miscellaneous", fileName = "Miscellaneous_", order = 3)]
public class MiscellaneousItemData : StackableItemData
{
    public override Item CreateItem()
    {
        return new MiscellaneousItem(this, 1);
    }

    public override Item CreateItem(int quantity)
    {
        return new MiscellaneousItem(this, quantity);
    }

    protected override ItemType GetItemType()
    {
        return ItemType.Miscellaneous;
    }
}
