using System;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class UI_Popup : UI_View, IPointerDownHandler
{
    public event Action Focused;
    public event Action Showed;
    public event Action Hided;

    [field: SerializeField]
    public bool IsHelper { get; private set; }

    [field: SerializeField]
    public bool IsSelfish { get; private set; }

    [field: SerializeField]
    public bool IgnoreSelfish { get; private set; }

    [field: SerializeField]
    public RectTransform Root { get; private set; }

    [field: SerializeField]
    public Vector3 DefaultPosition { get; private set; }

    protected override void Init()
    {
        base.Init();

        UIType = UIType.Popup;

        if (Root == null)
        {
            Root = transform.GetChild(0) as RectTransform;
        }

        Root.anchoredPosition = DefaultPosition;
    }

    protected virtual void Start()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        Showed?.Invoke();
    }

    private void OnDisable()
    {
        Hided?.Invoke();
    }

    private void OnDestroy()
    {
        ClearCallbacks();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Focused?.Invoke();
    }

    public void ClearCallbacks()
    {
        Focused = null;
        Showed = null;
        Hided = null;
    }
}
