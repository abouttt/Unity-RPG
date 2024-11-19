using UnityEngine;

public abstract class ConsumableItemData : StackableItemData, ILevelRequirement, ICooldownable
{
    [field: Header("Consumable Data")]
    [field: SerializeField]
    public int RequiredLevel { get; private set; } = 1;

    [field: SerializeField]
    public int ConsumptionQuantity { get; private set; } = 1;

    [field: SerializeField]
    public Cooldown Cooldown { get; private set; }

    protected override void Init()
    {
        SetItemType(ItemType.Consumable);
    }
}
