using UnityEngine;

[CreateAssetMenu(menuName = "Item/Misc", fileName = "Item_Misc_")]
public class MiscItemData : StackableItemData
{
    protected override void Init()
    {
        SetItemType(ItemType.Misc);
    }

    public override Item CreateItem()
    {
        return new MiscItem(this, 1);
    }

    public override StackableItem CreateItem(int quantity)
    {
        return new MiscItem(this, quantity);
    }
}
