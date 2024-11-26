using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_LootSubitem : MonoBehaviour
{
    public ItemData ItemData { get; private set; }
    public int Quantity { get; private set; }

    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private TextMeshProUGUI _itemNameText;

    [SerializeField]
    private TextMeshProUGUI _quantityText;

    [SerializeField]
    private Button _lootButton;

    private void Awake()
    {
        _lootButton.onClick.AddListener(() =>
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
            _itemImage.sprite = itemData.ItemImage;
            _itemNameText.text = itemData.ItemName;
        }

        Quantity = quantity;
        RefreshCountText();
    }

    private void RefreshCountText()
    {
        _quantityText.gameObject.SetActive(Quantity > 1);
        _quantityText.text = Quantity.ToString();
    }
}
