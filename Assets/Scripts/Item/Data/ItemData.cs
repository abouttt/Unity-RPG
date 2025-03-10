using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [field: Header("Basic Data")]
    [field: SerializeField]
    public string Id { get; private set; }

    [field: SerializeField]
    public string Name { get; private set; }

    [field: SerializeField, SpritePreview(100)]
    public Sprite Image { get; private set; }

    [field: SerializeField, ReadOnly]
    public ItemType Type { get; private set; }

    [field: SerializeField]
    public ItemRarity Rarity { get; private set; }

    [field: SerializeField, TextArea]
    public string Description { get; private set; }

    protected virtual void Awake()
    {
        if (string.IsNullOrEmpty(Id))
        {
            ResetId();
        }

        Type = GetItemType();
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

        return Id == other.Id;
    }

    protected abstract ItemType GetItemType();

    [ContextMenu("Reset Id")]
    private void ResetId()
    {
        Id = $"ITEM_{Type}_".ToUpper();
    }
}
