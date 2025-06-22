using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_Loot : UI_Popup, IConnectable<ItemInventory>
{
    public ItemInventory Context => _itemInventory;

    private ItemInventory _itemInventory;
    private FieldItem _fieldItem;
    private InputAction _interact;
    private readonly Dictionary<UI_LootSubitem, ItemData> _subitems = new();

    protected override void Init()
    {
        base.Init();

        _interact = Managers.Input.FindAction("Interact");

        GetText("LootAllText").text = $"[{Managers.Input.FindBindingPath("Interact")}] ¸ðµÎ È¹µæ";
        GetButton("LootAllButton").onClick.AddListener(AddAllItemToItemInventory);
        GetButton("CloseButton").onClick.AddListener(Managers.UI.Hide<UI_Loot>);

        Showed += () => _interact.performed += AddAllItemToItemInventoryInputAction;

        Hided += () =>
        {
            Clear();
            _interact.performed -= AddAllItemToItemInventoryInputAction;
        };
    }

    private void Update()
    {
        if (_fieldItem == null)
        {
            Managers.UI.Hide<UI_Loot>();
            return;
        }

        if (!_fieldItem.gameObject.activeSelf)
        {
            Managers.UI.Hide<UI_Loot>();
            return;
        }

        if (!_fieldItem.IsInteracted)
        {
            Managers.UI.Hide<UI_Loot>();
        }
    }

    public void SetFieldItem(FieldItem fieldItem)
    {
        foreach (var kvp in fieldItem.Items)
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

        _fieldItem = fieldItem;
    }

    public void AddItemToItemInventory(UI_LootSubitem subitem)
    {
        _fieldItem.RemoveItem(subitem.ItemData, subitem.Quantity);
        int quantity = _itemInventory.Add(subitem.ItemData, subitem.Quantity);
        if (quantity > 0)
        {
            subitem.SetItemData(subitem.ItemData, quantity);
        }
        else
        {
            RemoveSubitem(subitem);
        }
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

    private void CreateSubitem(ItemData itemData, int quantity)
    {
        Managers.Resource.InstantiateAsync<UI_LootSubitem>("UI_LootSubitem", subitem =>
        {
            subitem.SetItemData(itemData, quantity);
            subitem.SetButtonAction(() => AddItemToItemInventory(subitem));
            _subitems.Add(subitem, itemData);
        }
        , GetRectTransform("LootSubitems"), true);
    }

    private void RemoveSubitem(UI_LootSubitem subitem)
    {
        _subitems.Remove(subitem);
        Managers.Resource.Destroy(subitem.gameObject);
        if (_subitems.Count == 0)
        {
            Managers.UI.Hide<UI_Loot>();
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
            Managers.Resource.Destroy(kvp.Key.gameObject);
        }

        _subitems.Clear();

        if (_fieldItem != null)
        {
            _fieldItem.EndInteraction(null);
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

    private void OnDestroy()
    {
        Disconnect();
    }
}
