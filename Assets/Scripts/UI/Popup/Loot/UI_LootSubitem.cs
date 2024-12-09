using UnityEngine;

public class UI_LootSubitem : MonoBehaviour
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; private set; }

    private DataBinder _binder;

    private void Awake()
    {
        _binder = new(gameObject);
        _binder.GetButton("LootButton").onClick.AddListener(() =>
        {
            var lootPopup = UIManager.Get<UI_LootPopup>();
            lootPopup.AddItemToItemInventory(this);
        });
    }

    public void SetItemData(ItemData itemData, int quantity)
    {
        if (ItemData == null || !ItemData.Equals(itemData))
        {
            ItemData = itemData;
            _binder.GetImage("ItemImage").sprite = itemData.ItemImage;
            _binder.GetText("ItemNameText").text = itemData.ItemName;
        }

        Quantity = quantity;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        var quantityText = _binder.GetText("QuantityText");
        quantityText.gameObject.SetActive(Quantity > 1);
        quantityText.text = Quantity.ToString();
    }
}
