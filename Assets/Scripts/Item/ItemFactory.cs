using UnityEngine;

public static class ItemFactory
{
    public static Item CreateItem(ItemData itemData, int quantity = 1)
    {
        var item = new Item(itemData);

        if (item is StackableItem stackableItem)
        {
            stackableItem.Quantity = quantity;
        }

        return item;
    }
}
