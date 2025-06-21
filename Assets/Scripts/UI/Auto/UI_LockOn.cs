using UnityEngine;

public class UI_LockOn : UI_View, IConnectable<FieldOfView>
{
    public FieldOfView Context => _lockOnFov;

    private FieldOfView _lockOnFov;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    public void Connect(FieldOfView lockOnFov)
    {
        Disconnect();

        _lockOnFov = lockOnFov;
        lockOnFov.TargetChanged += SetTarget;
    }

    public void Disconnect()
    {
        if (_lockOnFov != null)
        {
            _lockOnFov.TargetChanged -= SetTarget;
            _lockOnFov = null;
        }
    }

    private void SetTarget(Transform target)
    {
        Get<UI_FollowWorldObject>("Root").Target = target;
        gameObject.SetActive(target != null);
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
