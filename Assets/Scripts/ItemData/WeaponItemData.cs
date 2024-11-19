using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon", fileName = "Weapon_", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    [field: SerializeField]
    public WeaponType WeaponType { get; private set; }

    protected override void Init()
    {
        base.Init();
        SetEquipmentType(EquipmentType.Weapon);
    }

    public override Item CreateItem()
    {
        return new WeaponItem(this);
    }
}
