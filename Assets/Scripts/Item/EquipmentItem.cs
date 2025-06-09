using UnityEngine;

public class EquipmentItem : Item
{
    public EquipmentItemData EquipmentData => Data as EquipmentItemData;

    public EquipmentItem(EquipmentItemData data)
        : base(data)
    {
    }
}
