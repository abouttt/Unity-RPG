using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon", fileName = "Weapon_", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    [Header("Weapon Data")]
    [field: SerializeField]
    public WeaponType WeaponType { get; private set; }

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }

    protected override EquipmentType GetEquipmentType()
    {
        return EquipmentType.Weapon;
    }
}
