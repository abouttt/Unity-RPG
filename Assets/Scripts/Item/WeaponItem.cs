using UnityEngine;

public class WeaponItem : EquipmentItem
{
    public WeaponItemData WeaponData => Data as WeaponItemData;

    public WeaponItem(WeaponItemData data)
        : base(data)
    { }
}
