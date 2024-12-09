using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public int Index { get; set; }
    public bool IsDragging { get; private set; }

    private DataBinder _binder;
    private Item _item;

    private void Awake()
    {
        _binder = new(gameObject);
        Clear();
    }

    private void OnDisable()
    {
        if (IsDragging)
        {
            IsDragging = false;
            _binder.GetImage("ItemImage").color = Color.white;
            _binder.GetImage("TempImage").gameObject.SetActive(false);
        }
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
            _binder.GetText("QuantityText").gameObject.SetActive(true);
            RefreshQuantityText(stackable);
        }

        if (item.Data is ICooldownable cooldownable)
        {
            _binder.Get<UI_CooldownImage>("CooldownImage").Connect(cooldownable.Cooldown);
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
                _binder.Get<UI_CooldownImage>("CooldownImage").Disconnect();
            }

            _item = null;
        }

        SetItemImage(null);
        _binder.GetText("QuantityText").gameObject.SetActive(false);
    }

    private void SetItemImage(Sprite image)
    {
        var itemImage = _binder.GetImage("ItemImage");
        var tempImage = _binder.GetImage("TempImage");
        itemImage.sprite = image;
        tempImage.sprite = image;
        itemImage.gameObject.SetActive(image != null);
        tempImage.gameObject.SetActive(false);
    }

    private void RefreshQuantityText(IStackable stackable)
    {
        _binder.GetText("QuantityText").text = stackable.Quantity.ToString();
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            eventData.pointerDrag = null;
            return;
        }

        var itemImage = _binder.GetImage("ItemImage");

        if (!itemImage.isActiveAndEnabled)
        {
            eventData.pointerDrag = null;
            return;
        }

        var tempImage = _binder.GetImage("TempImage");

        itemImage.color = new Color(1f, 1f, 1f, 0.3f);
        tempImage.gameObject.SetActive(true);
        tempImage.transform.SetParent(UIManager.Get<UI_TopCanvas>().transform);
        tempImage.transform.SetAsLastSibling();

        IsDragging = true;
    }

    public virtual void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            eventData.pointerDrag = null;
            return;
        }

        _binder.GetImage("TempImage").rectTransform.position = eventData.position;
    }

    public virtual void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            eventData.pointerDrag = null;
            return;
        }

        var itemImage = _binder.GetImage("ItemImage");
        var tempImage = _binder.GetImage("TempImage");
        itemImage.color = Color.white;
        tempImage.gameObject.SetActive(false);
        tempImage.transform.SetParent(transform);
        tempImage.rectTransform.position = transform.position;

        IsDragging = false;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var otherSlot))
        {
            if (_item == null && otherSlot._item is IStackable otherStackable && otherStackable.Quantity > 1)
            {
                var splitPopup = UIManager.Show<UI_ItemSplitPopup>();
                splitPopup.SetEvent(() =>
                {
                    UIManager.Get<UI_ItemInventoryPopup>().ItemInventory.SplitItem(otherSlot.Index, Index, splitPopup.Quantity);
                },
                $"[{otherSlot._item.Data.ItemName}] {GuideSettings.Instance.ItemSpliteText}", 1, otherStackable.Quantity);
            }
            else
            {
                UIManager.Get<UI_ItemInventoryPopup>().ItemInventory.MoveItem(otherSlot.Index, Index);
            }
        }
    }
}
