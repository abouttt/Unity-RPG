using UnityEngine;

public abstract class UI_View : MonoBehaviour
{
    [field: SerializeField]
    public UIType UIType { get; protected set; }

    public Canvas Canvas { get; private set; }

    private void Awake()
    {
        if (TryGetComponent(out Canvas canvas))
        {
            Canvas = canvas;
            Canvas.sortingOrder = (int)UIType;
        }
        else
        {
            Debug.LogWarning($"{name} ui view object does not have a Canvas component.");
        }

        Init();
    }

    protected abstract void Init();
}
