using UnityEngine;

public class ArmorItem : EquipmentItem
{
    public ArmorItemData ArmorData => Data as ArmorItemData;

    public ArmorItem(ArmorItemData data)
        : base(data)
    { }
}
