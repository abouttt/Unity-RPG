using UnityEngine;

public abstract class ConsumableItem : StackableItem, IConsumable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int quantity)
        : base(data, quantity)
    { }

    public bool Consume()
    {
        if (!CanConsume())
        {
            return false;
        }

        OnConsumed();
        Quantity -= ConsumableData.ConsumptionQuantity;

        return true;
    }

    public bool CanConsume()
    {
        if (Quantity < ConsumableData.ConsumptionQuantity)
        {
            return false;
        }

        return true;
    }

    protected abstract void OnConsumed();
}
