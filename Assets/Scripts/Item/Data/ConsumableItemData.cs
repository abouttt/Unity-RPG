using UnityEngine;

public abstract class ConsumableItemData : StackableItemData, ILevelRequirement
{
    [field: Header("Consumable Data")]
    [field: SerializeField]
    public int RequiredLevel { get; private set; } = 1;

    [field: SerializeField]
    public int ConsumptionQuantity { get; private set; } = 1;

    protected override void Init()
    {
        SetItemType(ItemType.Consumable);
    }
}
