using UnityEngine;

public class UI_ItemInventoryPopup : UI_Popup, IConnectable<ItemInventory>
{
    public ItemInventory ItemInventory { get; private set; }

    private DataBinder _binder;
    private UI_ItemSlot[] _slots;

    protected override void Init()
    {
        base.Init();

        _binder = new(gameObject);
        _binder.GetButton("CloseButton").onClick.AddListener(UIManager.Close<UI_ItemInventoryPopup>);

        UIManager.Register(this);
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

        ItemInventory = itemInventory;
        itemInventory.InventoryChanged += RefreshSlot;
        CreateSlots(itemInventory.Capacity, _binder.GetRectTransform("ItemSlots"));
    }

    public void Disconnect()
    {
        if (ItemInventory != null)
        {
            ItemInventory.InventoryChanged -= RefreshSlot;
            ItemInventory = null;
        }
    }

    private void RefreshSlot(Item item, int index)
    {
        _slots[index].Refresh(item);
    }

    private void CreateSlots(int count, Transform parent)
    {
        for (int index = 0; index < count; index++)
        {
            ResourceManager.InstantiateAsync<UI_ItemSlot>("UI_ItemSlot", itemSlot =>
            {
                itemSlot.Index = index;
            }
            , parent);
        }

        _slots = parent.GetComponentsInChildren<UI_ItemSlot>();
    }
}
