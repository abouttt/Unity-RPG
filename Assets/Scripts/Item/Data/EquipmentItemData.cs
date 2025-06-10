using UnityEngine;

public enum EquipmentType
{
    Helmet,
    Chest,
    Pants,
    Boots,
    Weapon,
    Shield,
}

[CreateAssetMenu(menuName = "Item/Equipment", fileName = "Item_Equipment_")]
public class EquipmentItemData : ItemData, ILevelRequirement
{
    [field: Header("Equipment Data")]
    [field: SerializeField]
    public EquipmentType EquipmentType { get; private set; }

    [field: SerializeField]
    public int RequiredLevel { get; private set; } = 1;

    [field: SerializeField]
    public GameObject Prefab { get; private set; }

    protected override void Init()
    {
        SetItemType(ItemType.Equipment);
    }

    public override Item CreateItem()
    {
        return new EquipmentItem(this);
    }
}
