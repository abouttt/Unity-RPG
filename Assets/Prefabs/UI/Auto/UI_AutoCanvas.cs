using UnityEngine;

public class UI_AutoCanvas : UI_View
{
    public UI_LockOn LockOnUI => _lockOnUI;

    [SerializeField]
    private UI_LockOn _lockOnUI;

    protected override void Init()
    {
        Managers.UI.Register(this);
    }
}
