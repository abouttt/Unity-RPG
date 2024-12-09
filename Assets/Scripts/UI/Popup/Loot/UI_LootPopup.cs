using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_LootPopup : UI_Popup, IConnectable<ItemInventory>
{
    [SerializeField]
    private float _trackingDistance;

    private DataBinder _binder;
    private ItemInventory _itemInventory;
    private FieldItem _fieldItem;
    private InputAction _interact;
    private readonly Dictionary<UI_LootSubitem, ItemData> _subitems = new();

    protected override void Init()
    {
        base.Init();

        _interact = InputManager.GetAction("Interact");

        _binder = new(gameObject);
        _binder.GetText("LootAllText").text = $"[{InputManager.GetBindingPath("Interact")}] ¸ðµÎ È¹µæ";
        _binder.GetButton("LootAllButton").onClick.AddListener(AddAllItemToItemInventory);
        _binder.GetButton("CloseButton").onClick.AddListener(UIManager.Close<UI_LootPopup>);

        Showed += () => _interact.performed += AddAllItemToItemInventoryInputAction;

        Closed += () =>
        {
            Clear();
            _interact.performed -= AddAllItemToItemInventoryInputAction;
        };

        UIManager.Register(this);
    }

    private void Update()
    {
        if (_fieldItem == null)
        {
            UIManager.Close<UI_LootPopup>();
            return;
        }

        if (!_fieldItem.gameObject.activeSelf)
        {
            UIManager.Close<UI_LootPopup>();
            return;
        }

        TrackingFieldItem();
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void Connect(ItemInventory itemInventory)
    {
        if (itemInventory != null)
        {
            _itemInventory = itemInventory;
        }
    }

    public void Disconnect()
    {
        _itemInventory = null;
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        _fieldItem = fieldItem;

        foreach (var kvp in _fieldItem.Items)
        {
            if (kvp.Key is StackableItemData stackableData)
            {
                int quantity = kvp.Value;
                while (quantity > 0)
                {
                    CreateSubitem(kvp.Key, Mathf.Clamp(quantity, quantity, stackableData.MaxQuantity));
                    quantity -= stackableData.MaxQuantity;
                }
            }
            else
            {
                for (int i = 0; i < kvp.Value; i++)
                {
                    CreateSubitem(kvp.Key, 1);
                }
            }
        }
    }

    public void AddItemToItemInventory(UI_LootSubitem subitem)
    {
        _fieldItem.RemoveItem(subitem.ItemData, subitem.Quantity);
        int quantity = _itemInventory.AddItem(subitem.ItemData, subitem.Quantity);
        if (quantity > 0)
        {
            subitem.SetItemData(subitem.ItemData, quantity);
        }
        else
        {
            RemoveSubitem(subitem);
        }
    }

    private void CreateSubitem(ItemData itemData, int count)
    {
        ResourceManager.InstantiateAsync<UI_LootSubitem>("UI_LootSubitem", subitem =>
        {
            subitem.SetItemData(itemData, count);
            _subitems.Add(subitem, itemData);
        }
        , _binder.GetRectTransform("LootSubitems"), true);
    }

    private void RemoveSubitem(UI_LootSubitem subitem)
    {
        _subitems.Remove(subitem);
        ResourceManager.Destroy(subitem.gameObject);
        if (_subitems.Count == 0)
        {
            UIManager.Close<UI_LootPopup>();
        }
    }

    private void TrackingFieldItem()
    {
        if (_trackingDistance < Vector3.Distance(_itemInventory.gameObject.transform.position, _fieldItem.transform.position))
        {
            UIManager.Close<UI_LootPopup>();
        }
    }

    private void AddAllItemToItemInventory()
    {
        for (int i = _subitems.Count - 1; i >= 0; i--)
        {
            AddItemToItemInventory(_subitems.ElementAt(i).Key);
        }
    }

    private void Clear()
    {
        foreach (var kvp in _subitems)
        {
            ResourceManager.Destroy(kvp.Key.gameObject);
        }

        _subitems.Clear();

        if (_fieldItem != null)
        {
            _fieldItem.StopInteract();
            _fieldItem = null;
        }
    }

    private void AddAllItemToItemInventoryInputAction(InputAction.CallbackContext context)
    {
        if (context.ReadValueAsButton())
        {
            AddAllItemToItemInventory();
        }
    }
}
