using UnityEngine;

[CreateAssetMenu(menuName = "Item/Armor", fileName = "Armor_", order = 0)]
public class ArmorItemData : EquipmentItemData
{
    [field: SerializeField]
    public ArmorType ArmorType { get; private set; }

    protected override void Init()
    {
        base.Init();
        SetEquipmentType(EquipmentType.Armor);
    }

    public override Item CreateItem()
    {
        return new ArmorItem(this);
    }
}
