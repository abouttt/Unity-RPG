using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor", fileName = "Armor_", order = 0)]
public class ArmorItemData : EquipmentItemData
{
    [Header("Armor Data")]
    [field: SerializeField]
    public ArmorType ArmorType { get; private set; }

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }

    protected override EquipmentType GetEquipmentType()
    {
        return EquipmentType.Armor;
    }
}
