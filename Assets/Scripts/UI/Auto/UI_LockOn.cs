using UnityEngine;

public class UI_LockOn : UI_View, IConnectable<FieldOfView>
{
    public FieldOfView Context => _lockOnFovRef;

    private FieldOfView _lockOnFovRef;

    protected override void Init()
    {
        base.Init();
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void Connect(FieldOfView lockOnFov)
    {
        Disconnect();

        _lockOnFovRef = lockOnFov;
        lockOnFov.TargetChanged += SetTarget;
    }

    public void Disconnect()
    {
        if (_lockOnFovRef != null)
        {
            _lockOnFovRef.TargetChanged -= SetTarget;
            _lockOnFovRef = null;
        }
    }

    private void SetTarget(Transform target)
    {
        Get<UI_FollowWorldObject>("Body").Target = target;
        gameObject.SetActive(target != null);
    }
}
