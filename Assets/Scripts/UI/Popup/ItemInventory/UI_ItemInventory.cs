using UnityEngine;
using UnityEngine.EventSystems;

public class UI_ItemInventory : UI_Popup, IConnectable<ItemInventory>
{
    public ItemInventory Context => _itemInventory;

    private ItemInventory _itemInventory;
    private UI_ItemSlot[] _itemSlots;
    private UI_ItemSlot _draggedItemSlot;

    protected override void Init()
    {
        base.Init();

        GetButton("CloseButton").onClick.AddListener(Managers.UI.Hide<UI_ItemInventory>);
        GetImage("DraggedItemIconImage").gameObject.SetActive(false);

        Hided += () =>
        {
            if (_draggedItemSlot != null)
            {
                _draggedItemSlot.CancelDrag();
            }
        };
    }

    public void Connect(ItemInventory itemInventory)
    {
        if (itemInventory == null)
        {
            return;
        }

        Disconnect();

        _itemInventory = itemInventory;
        itemInventory.Changed += RefreshSlot;
        InstantiateSlots(itemInventory.Capacity, GetRectTransform("ItemSlots"));
    }

    public void Disconnect()
    {
        if (_itemInventory != null)
        {
            _itemInventory.Changed -= RefreshSlot;
            _itemInventory = null;
            DestroySlots();
        }
    }

    private void RefreshSlot(Item item, int index)
    {
        _itemSlots[index].Refresh(item);
    }

    private void BeginDragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        var draggedItemImage = GetImage("DraggedItemIconImage");
        draggedItemImage.sprite = itemSlot.Item.Data.Icon;
        draggedItemImage.gameObject.SetActive(true);
        _draggedItemSlot = itemSlot;
    }

    private void DragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DraggedItemIconImage").rectTransform.position = eventData.position;
    }

    private void EndDragItemSlot(UI_ItemSlot itemSlot, PointerEventData eventData)
    {
        GetImage("DraggedItemIconImage").gameObject.SetActive(false);
        _draggedItemSlot = null;
    }

    private void DropItemSlot(UI_ItemSlot fromSlot, UI_ItemSlot toSlot, PointerEventData eventData)
    {
        if (fromSlot.Item == null && toSlot.Item is IStackable otherStackable && otherStackable.Quantity > 1)
        {
            var splitPopup = Managers.UI.Show<UI_ItemSplit>();
            splitPopup.SetEvent(() =>
            {
                _itemInventory.Split(toSlot.Index, fromSlot.Index, splitPopup.Quantity);
            },
            $"[{toSlot.Item.Data.Name}] {Settings.Guide.ItemSpliteText}", 1, otherStackable.Quantity);
        }
        else
        {
            _itemInventory.Move(fromSlot.Index, toSlot.Index);
        }
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

    private void OnDestroy()
    {
        Disconnect();
    }
}
