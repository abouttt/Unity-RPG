using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Background : UI_View, IPointerDownHandler, IDropHandler
{
    protected override void Init()
    {

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
                eventData.pointerDrag = null;
                return;
            }

            OnDropItemSlot(itemSlot);
        }
    }

    private void OnDropItemSlot(UI_ItemSlot itemSlot)
    {
        var itemInventory = UIManager.Get<UI_ItemInventory>().Context;
        var item = itemInventory.GetItem<Item>(itemSlot.Index);
        string guideText = $"[{item.Data.ItemName}] {GuideSettings.Instance.DestroyText}";
        UIManager.Show<UI_Confirmation>().SetEvent(() =>
        {
            itemInventory.RemoveItem(itemSlot.Index);
        },
        guideText);
    }
}
