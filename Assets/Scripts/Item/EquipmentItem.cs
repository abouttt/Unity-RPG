using UnityEngine;

public abstract class EquipmentItem : Item
{
    public EquipmentItemData EquipmentData => Data as EquipmentItemData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    { }
}
