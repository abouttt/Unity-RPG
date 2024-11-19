using UnityEngine;

public abstract class EquipmentItemData : ItemData, ILevelRequirement
{
    [field: Header("Equipment Data")]
    [field: SerializeField, ReadOnly]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public int RequiredLevel { get; private set; } = 1;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    protected override void Init()
    {
        SetItemType(ItemType.Equipment);
    }

    protected void SetEquipmentType(EquipmentType equipmentType)
    {
        EquipmentType = equipmentType;
    }
}
