using UnityEngine;

public abstract class UI_View : MonoBehaviour
{
    [field: SerializeField]
    public UIType UIType { get; protected set; }

    public Canvas Canvas => _canvas;

    private Canvas _canvas;

    private void Awake()
    {
        if (TryGetComponent(out _canvas))
        {
            _canvas.sortingOrder = (int)UIType;
        }
        else
        {
            Debug.LogWarning($"{name} ui view object does not have a Canvas component.");
        }

        Init();
    }

    protected abstract void Init();
}
