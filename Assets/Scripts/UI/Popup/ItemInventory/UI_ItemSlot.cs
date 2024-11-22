using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int Index { get; set; }

    [SerializeField]
    private Image _itemImage;

    [SerializeField]
    private Image _tempImage;

    [SerializeField]
    private UI_CooldownImage _cooldownImage;

    [SerializeField]
    private TextMeshProUGUI _quantityText;

    private Item _item;

    private void Awake()
    {
        Clear();
    }

    public void Refresh(Item item)
    {
        Clear();

        if (item == null)
        {
            return;
        }

        SetItemImage(item.Data.ItemImage);

        if (item is IStackable stackable)
        {
            stackable.StackChanged += RefreshQuantityText;
            _quantityText.gameObject.SetActive(true);
            RefreshQuantityText(stackable);
        }

        if (item.Data is ICooldownable cooldownable)
        {
            _cooldownImage.Connect(cooldownable.Cooldown);
        }

        _item = item;
    }

    private void Clear()
    {
        if (_item != null)
        {
            if (_item is IStackable stackable)
            {
                stackable.StackChanged -= RefreshQuantityText;
            }

            if (_item.Data is ICooldownable)
            {
                _cooldownImage.Disconnect();
            }

            _item = null;
        }

        SetItemImage(null);
        _quantityText.gameObject.SetActive(false);
    }

    private void SetItemImage(Sprite image)
    {
        _itemImage.sprite = image;
        _tempImage.sprite = image;
        _itemImage.gameObject.SetActive(image != null);
        _tempImage.gameObject.SetActive(false);
    }

    private void RefreshQuantityText(IStackable stackable)
    {
        _quantityText.text = stackable.Quantity.ToString();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (!_itemImage.isActiveAndEnabled)
        {
            eventData.pointerDrag = null;
            return;
        }

        _itemImage.color = new Color(1f, 1f, 1f, 0.3f);
        _tempImage.gameObject.SetActive(true);
        _tempImage.transform.SetParent(UIManager.Get<UI_TopCanvas>().transform);
        _tempImage.transform.SetAsLastSibling();
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        _tempImage.rectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        _itemImage.color = Color.white;
        _tempImage.gameObject.SetActive(false);
        _tempImage.transform.SetParent(transform);
        _tempImage.rectTransform.position = transform.position;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherSlot))
        {
            UIManager.Get<UI_ItemInventoryPopup>().ItemInventory.MoveItem(otherSlot.Index, Index);
        }
    }
}
