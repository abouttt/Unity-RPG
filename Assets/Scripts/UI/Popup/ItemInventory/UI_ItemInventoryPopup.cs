using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemInventoryPopup : UI_Popup, IConnectable<ItemInventory>
{
    public ItemInventory Context => _itemInventoryRef;

    private ItemInventory _itemInventoryRef;
    private UI_ItemSlot[] _itemSlots;
    private UI_ItemSlot _draggedItemSlot;

    protected override void Init()
    {
        base.Init();
        GetButton("CloseButton").onClick.AddListener(UIManager.Hide<UI_ItemInventoryPopup>);
        GetImage("DragItemImage").gameObject.SetActive(false);

        Hided += () =>
        {
            if (_draggedItemSlot != null)
            {
                _draggedItemSlot.CancelDrag();
            }
        };
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void Connect(ItemInventory itemInventory)
    {
        if (itemInventory == null)
        {
            return;
        }

        Disconnect();

        _itemInventoryRef = itemInventory;
        itemInventory.InventoryChanged += RefreshSlot;
        InstantiateSlots(itemInventory.Capacity, GetRectTransform("ItemSlots"));
    }

    public void Disconnect()
    {
        if (_itemInventoryRef != null)
        {
            _itemInventoryRef.InventoryChanged -= RefreshSlot;
            _itemInventoryRef = null;
            DestroySlots();
        }
    }

    private void RefreshSlot(Item item, int index)
    {
        _itemSlots[index].Refresh(item);
    }

    private void BeginDragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        var dragItemImage = GetImage("DragItemImage");
        dragItemImage.gameObject.SetActive(true);
        dragItemImage.sprite = itemSlot.ItemRef.Data.ItemImage;
        _draggedItemSlot = itemSlot;
    }

    private void DragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DragItemImage").rectTransform.position = eventData.position;
    }

    private void EndDragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DragItemImage").gameObject.SetActive(false);
        _draggedItemSlot = null;
    }

    private void DropItemSlot(UI_ItemSlot itemSlot, UI_ItemSlot droppedItemSlot, PointerEventData eventData)
    {
        _itemInventoryRef.MoveItem(droppedItemSlot.Index, itemSlot.Index);
    }

    private void InstantiateSlots(int count, Transform parent)
    {
        for (int index = 0; index < count; index++)
        {
            ResourceManager.InstantiateAsync<UI_ItemSlot>("UI_ItemSlot", itemSlot =>
            {
                itemSlot.Index = index;
                itemSlot.BeginDraged += BeginDragItemSlot;
                itemSlot.Draged += DragItemSlot;
                itemSlot.EndDraged += EndDragItemSlot;
                itemSlot.Dropped += DropItemSlot;
            }
            , parent);
        }

        _itemSlots = parent.GetComponentsInChildren<UI_ItemSlot>();
    }

    private void DestroySlots()
    {
        foreach (var itemSlot in _itemSlots)
        {
            itemSlot.Index = -1;
            itemSlot.BeginDraged -= BeginDragItemSlot;
            itemSlot.Draged -= DragItemSlot;
            itemSlot.EndDraged -= EndDragItemSlot;
            itemSlot.Dropped -= DropItemSlot;
            Destroy(itemSlot.gameObject);
        }
    }
}
