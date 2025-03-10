using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemInventory : UI_Popup, IConnectable<ItemInventory>
{
    public ItemInventory Context => _itemInventoryRef;

    private ItemInventory _itemInventoryRef;
    private UI_ItemSlot[] _itemSlots;
    private UI_ItemSlot _draggedItemSlot;

    protected override void Init()
    {
        base.Init();

        GetButton("CloseButton").onClick.AddListener(Managers.UI.Hide<UI_ItemInventory>);
        GetImage("DraggedItemImage").gameObject.SetActive(false);

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
        var dragItemImage = GetImage("DraggedItemImage");
        dragItemImage.gameObject.SetActive(true);
        dragItemImage.sprite = itemSlot.ItemRef.Data.Image;
        _draggedItemSlot = itemSlot;
    }

    private void DragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DraggedItemImage").rectTransform.position = eventData.position;
    }

    private void EndDragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DraggedItemImage").gameObject.SetActive(false);
        _draggedItemSlot = null;
    }

    private void DropItemSlot(UI_ItemSlot itemSlot, UI_ItemSlot droppedItemSlot, PointerEventData eventData)
    {
        _itemInventoryRef.Move(droppedItemSlot.Index, itemSlot.Index);
    }

    private void InstantiateSlots(int count, Transform parent)
    {
        _itemSlots = new UI_ItemSlot[count];

        for (int index = 0; index < count; index++)
        {
            int currentIndex = index;
            Managers.Resource.InstantiateAsync<UI_ItemSlot>("UI_ItemSlot", itemSlot =>
            {
                itemSlot.Index = currentIndex;
                itemSlot.BeginDraged += BeginDragItemSlot;
                itemSlot.Draged += DragItemSlot;
                itemSlot.EndDraged += EndDragItemSlot;
                itemSlot.Dropped += DropItemSlot;
                _itemSlots[currentIndex] = itemSlot;
            }
            , parent);
        }
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

        _itemSlots = null;
    }
}
