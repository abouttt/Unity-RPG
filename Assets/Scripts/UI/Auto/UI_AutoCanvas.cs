using UnityEngine;

public class UI_AutoCanvas : UI_View
{
    public UI_LockOn LockOnUI => _lockOnUI;
    public UI_Interactor InteractorUI => _interactorUI;

    [SerializeField]
    private UI_LockOn _lockOnUI;

    [SerializeField]
    private UI_Interactor _interactorUI;

    protected override void Init()
    {
        UIManager.Register(this);
    }
}
