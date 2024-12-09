using UnityEngine;
using UnityEngine.EventSystems;

public class UI_BackgroundCanvas : UI_View, IPointerDownHandler, IDropHandler
{
    protected override void Init()
    {
        UIManager.Register(this);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        InputManager.CursorLocked = true;
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag.TryGetComponent<UI_ItemSlot>(out var itemSlot))
        {
            if (!itemSlot.IsDragging)
            {
                return;
            }

            OnDropItemSlot(itemSlot);
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        var itemInventory = UIManager.Get<UI_ItemInventoryPopup>().ItemInventory;
        var item = itemInventory.GetItem<Item>(itemSlot.Index);
        string guideText = $"[{item.Data.ItemName}] {GuideSettings.Instance.DestroyText}";
        UIManager.Show<UI_ConfirmationPopup>().SetEvent(() =>
        {
            itemInventory.RemoveItem(itemSlot.Index);
        },
        guideText);
    }
}
