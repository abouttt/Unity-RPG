using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor", fileName = "Armor_", order = 0)]
public class ArmorItemData : EquipmentItemData
{
    [Header("Armor Data")]
    [field: SerializeField]
    public ArmorType ArmorType { get; private set; }

    protected override void Awake()
    {
        base.Awake();
        EquipmentType = EquipmentType.Armor;
    }
}
