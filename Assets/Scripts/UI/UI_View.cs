using UnityEngine;

public abstract class UI_View : MonoBehaviour
{
    [field: SerializeField]
    public UIType UIType { get; protected set; }

    protected DataBinder _dataBinder;

    private void Awake()
    {
        _dataBinder = new(gameObject);
        Init();
    }

    protected abstract void Init();
}
