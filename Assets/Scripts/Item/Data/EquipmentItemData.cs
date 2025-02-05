using UnityEngine;

public abstract class EquipmentItemData : ItemData, ILevelRequirement
{
    [field: Header("Equipment Data")]
    [field: SerializeField, ReadOnly]
    public EquipmentType EquipmentType { get; protected set; }

    [field: SerializeField]
    public int RequiredLevel { get; private set; } = 1;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        ItemType = ItemType.Equipment;
    }
}
