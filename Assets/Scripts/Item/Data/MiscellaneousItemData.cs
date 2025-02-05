using UnityEngine;

[CreateAssetMenu(menuName = "Item/Miscellaneous", fileName = "Miscellaneous_", order = 3)]
public class MiscellaneousItemData : StackableItemData
{
    protected override void Awake()
    {
        base.Awake();
        ItemType = ItemType.Miscellaneous;
    }
}
