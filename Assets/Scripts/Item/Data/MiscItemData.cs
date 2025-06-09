using UnityEngine;

[CreateAssetMenu(menuName = "Item/Misc", fileName = "Item_Misc_")]
public class MiscItemData : StackableItemData
{
    protected override void Init()
    {
        SetItemType(ItemType.Misc);
    }
}
