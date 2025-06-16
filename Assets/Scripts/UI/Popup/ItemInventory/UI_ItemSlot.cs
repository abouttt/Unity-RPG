using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemSlot : UI_Base, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    public event Action<UI_ItemSlot, PointerEventData> BeginDraged;
    public event Action<UI_ItemSlot, PointerEventData> Draged;
    public event Action<UI_ItemSlot, PointerEventData> EndDraged;
    public event Action<UI_ItemSlot, UI_ItemSlot, PointerEventData> Dropped;

    public int Index { get; set; } = -1;
    public Item ItemRef { get; private set; }
    public bool IsDragging { get; private set; }

    protected override void Init()
    {
        Clear();
    }

    public void Refresh(Item item)
    {
        if (ItemRef == item)
        {
            return;
        }
        else if (ItemRef != null)
        {
            Clear();
        }

        if (item == null)
        {
            return;
        }

        SetItemImage(item.Data.Icon);

        if (item is IStackable stackable)
        {
            stackable.StackChanged += RefreshQuantityText;
            GetText("QuantityText").gameObject.SetActive(true);
            RefreshQuantityText(stackable);
        }

        ItemRef = item;
    }

    public void CancelDrag()
    {
        if (IsDragging)
        {
            IsDragging = false;
            GetImage("ItemImage").color = Color.white;
            EndDraged?.Invoke(this, null);
        }
    }

    private void Clear()
    {
        SetItemImage(null);

        if (ItemRef is IStackable stackable)
        {
            stackable.StackChanged -= RefreshQuantityText;
        }

        GetText("QuantityText").gameObject.SetActive(false);
        ItemRef = null;
    }

    private void SetItemImage(Sprite image)
    {
        var itemImage = GetImage("ItemImage");
        itemImage.sprite = image;
        itemImage.gameObject.SetActive(image != null);
    }

    private void RefreshQuantityText(IStackable stackable)
    {
        if (stackable.Quantity > 1)
        {
            GetText("QuantityText").text = stackable.Quantity.ToString();
        }
        else
        {
            GetText("QuantityText").text = null;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            eventData.pointerDrag = null;
            return;
        }

        if (ItemRef == null)
        {
            eventData.pointerDrag = null;
            return;
        }

        IsDragging = true;
        GetImage("ItemImage").color = new Color(1f, 1f, 1f, 0.3f);
        BeginDraged?.Invoke(this, eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            eventData.pointerDrag = null;
            return;
        }

        Draged?.Invoke(this, eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!IsDragging)
        {
            eventData.pointerDrag = null;
            return;
        }

        CancelDrag();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == gameObject)
        {
            return;
        }

        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var droppedItemSlot))
        {
            if (!droppedItemSlot.IsDragging)
            {
                eventData.pointerDrag = null;
                return;
            }

            Dropped?.Invoke(this, droppedItemSlot, eventData);
        }
    }
}
