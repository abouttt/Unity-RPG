using System;
using UnityEngine;

public class UI_LootSubitem : UI_Base
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; private set; }

    protected override void Init()
    {
    }

    public void SetItemData(ItemData itemData, int quantity)
    {
        if (ItemData == null || !ItemData.Equals(itemData))
        {
            ItemData = itemData;
            GetImage("ItemIconImage").sprite = itemData.Icon;
            GetText("ItemNameText").text = itemData.Name;
        }

        Quantity = quantity;
        RefreshQuantityText();
    }

    public void SetOnButtonClickEvent(Action callback)
    {
        var lootButton = GetButton("LootButton");
        lootButton.onClick.RemoveAllListeners();
        lootButton.onClick.AddListener(() => callback.Invoke());
    }

    private void RefreshQuantityText()
    {
        var quantityText = GetText("QuantityText");
        quantityText.gameObject.SetActive(Quantity > 1);
        quantityText.text = Quantity.ToString();
    }
}
