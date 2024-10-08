using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [field: Header("Basic Data")]
    [field: SerializeField]
    public string ItemId { get; private set; }

    [field: SerializeField]
    public string ItemName { get; private set; }

    [field: SerializeField, SpritePreview(100)]
    public Sprite ItemImage { get; private set; }

    [field: SerializeField, ReadOnly]
    public ItemType ItemType { get; private set; }

    [field: SerializeField]
    public ItemRarity ItemRarity { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    private void Awake()
    {
        Init();
        if (string.IsNullOrEmpty(ItemId))
        {
            ResetId();
        }
    }

    public abstract Item CreateItem();

    public bool Equals(ItemData other)
    {
        if (other == null)
        {
            return false;
        }

        if (GetType() != other.GetType())
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return ItemId.Equals(other.ItemId);
    }

    protected abstract void Init();

    protected void SetItemType(ItemType itemType)
    {
        ItemType = itemType;
    }

    [ContextMenu("Reset Id")]
    private void ResetId()
    {
        ItemId = $"ITEM_{ItemType}_".ToUpper();
    }
}
