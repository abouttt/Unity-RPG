using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Background : UI_View, IPointerDownHandler, IDropHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Managers.Input.CursorLocked = true;
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
        var itemInventory = Managers.UI.Get<UI_ItemInventory>().Context;
        var item = itemInventory.Get<Item>(itemSlot.Index);
        string guideText = $"[{item.Data.Name}] {Settings.Guide.DestroyItemText}";
        Managers.UI.Show<UI_Confirmation>().SetEvent(() =>
        {
            itemInventory.Remove(itemSlot.Index);
        },
        guideText);
    }
}
