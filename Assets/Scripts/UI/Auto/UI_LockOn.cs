using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_LockOn : MonoBehaviour, IConnectable<FieldOfView>
{
    private FieldOfView _lockOnFov;
    private UI_FollowWorldObject _followTarget;

    private void Awake()
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
        _followTarget.Target = target;
        gameObject.SetActive(target != null);
    }
}
