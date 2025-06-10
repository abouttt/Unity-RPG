using UnityEngine;

public abstract class ConsumableItem : StackableItem, IConsumable
{
    public ConsumableItemData ConsumableData => Data as ConsumableItemData;

    public ConsumableItem(ConsumableItemData data, int quantity)
        : base(data, quantity)
    {
    }

    public bool Consume(GameObject target)
    {
        if (!CanConsume())
        {
            return false;
        }

        OnConsumed(target);
        Quantity -= ConsumableData.ConsumptionQuantity;
        Managers.Cooldown.ApplyCooldown(ConsumableData);

        return true;
    }

    public virtual bool CanConsume()
    {
        if (Quantity < ConsumableData.ConsumptionQuantity)
        {
            return false;
        }

        if (Managers.Cooldown.IsOnCooldown(ConsumableData))
        {
            return false;
        }

        return true;
    }

    protected abstract void OnConsumed(GameObject target);
}
