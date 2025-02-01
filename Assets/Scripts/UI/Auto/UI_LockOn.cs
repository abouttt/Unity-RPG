using UnityEngine;

public class UI_LockOn : UI_View, IConnectable<FieldOfView>
{
    public FieldOfView Context => _lockOnFovRef;

    private FieldOfView _lockOnFovRef;
    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        _followTarget = GetComponent<UI_FollowWorldObject>();
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
        _followTarget.Target = target;
        gameObject.SetActive(target != null);
    }
}
