using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon", fileName = "Weapon_", order = 1)]
public class WeaponItemData : EquipmentItemData
{
    [Header("Weapon Data")]
    [field: SerializeField]
    public WeaponType WeaponType { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Weapon;
    }
}
