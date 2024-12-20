using UnityEngine;

public abstract class UI_View : MonoBehaviour
{
    [field: SerializeField]
    public UIType UIType { get; protected set; }

    [field: SerializeField]
    public bool IsValidForUISettings { get; private set; } = true;

    protected DataBinder _binder;

    private void Awake()
    {
        _binder = new(gameObject);
        Init();
    }

    protected abstract void Init();
}
