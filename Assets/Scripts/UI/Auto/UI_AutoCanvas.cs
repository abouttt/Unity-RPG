using UnityEngine;

public class UI_AutoCanvas : UI_View
{
    [field: SerializeField]
    public UI_LockOn LockOnUI { get; private set; }

    [field: SerializeField]
    public UI_Interactor InteractorUI { get; private set; }

    protected override void Init()
    {
        UIManager.Register(this);
    }
}
