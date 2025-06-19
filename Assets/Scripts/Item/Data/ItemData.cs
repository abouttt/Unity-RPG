using UnityEngine;

public enum ItemType
{
    Equipment,
    Consumable,
    Misc,
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary,
}

public abstract class ItemData : ScriptableObject
{
    [field: Header("Basic Data")]
    [field: SerializeField]
    public string Id { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField, SpritePreview(100)]
    public Sprite Icon { get; private set; }

    [field: SerializeField, ReadOnly]
    public ItemType Type { get; private set; }

    [field: SerializeField]
    public ItemRarity Rarity { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    [field: SerializeField]
    public bool IsDestructible { get; private set; } = true;

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

        return Id == other.Id;
    }

    protected abstract void Init();

    protected void SetItemType(ItemType itemType)
    {
        Type = itemType;
    }

    protected string GetIdConvention()
    {
        return $"ITEM_{Type}_".ToUpper();
    }

    [ContextMenu("Reset Id")]
    private void ResetId()
    {
        Id = GetIdConvention();
    }

    private void Reset()
    {
        Init();
        ResetId();
    }
}
