using UnityEngine;

[RequireComponent(typeof(FieldOfView))]
public class FieldOfViewTracker : MonoBehaviour
{
    [SerializeField]
    private bool _distance = true;

    [SerializeField]
    private bool _obstacle = true;

    [SerializeField]
    private bool _horizontal = true;

    [SerializeField]
    private bool _vertical = true;

    [SerializeField]
    private float _horizontalClamp;

    [SerializeField]
    private float _verticalClamp;

    private FieldOfView _fieldOfView;

    private void Awake()
    {
        _fieldOfView = GetComponent<FieldOfView>();
    }

    private void LateUpdate()
    {
        TrackingTarget();
    }

    public void TrackingTarget()
    {
        if (_fieldOfView == null)
        {
            return;
        }

        if (!_fieldOfView.HasTarget)
        {
            return;
        }

        if (IsOutOfRange() ||
            IsObstructed() ||
            IsOutOfAngle())
        {
            _fieldOfView.Target = null;
        }
    }

    private bool IsOutOfRange()
    {
        return _distance && Vector3.Distance(_fieldOfView.transform.position, _fieldOfView.Target.position) > _fieldOfView.ViewRadius;
    }

    private bool IsObstructed()
    {
        return _obstacle && Physics.Linecast(_fieldOfView.transform.position, _fieldOfView.Target.position, _fieldOfView.ObstacleLayer);
    }

    private bool IsOutOfAngle()
    {
        var toTarget = (_fieldOfView.Target.position - transform.position).normalized;

        bool outOfHorizontal = false;
        bool outOfVertical = false;

        if (_horizontal)
        {
            var flatToTarget = Vector3.ProjectOnPlane(toTarget, Vector3.up).normalized;
            float horizontalAngle = Vector3.SignedAngle(transform.forward, flatToTarget, Vector3.up);
            outOfHorizontal = Mathf.Abs(horizontalAngle) > _horizontalClamp;
        }

        if (_vertical)
        {
            var verticalToTarget = Vector3.ProjectOnPlane(toTarget, transform.right).normalized;
            float verticalAngle = Vector3.SignedAngle(transform.forward, verticalToTarget, transform.right);
            outOfVertical = Mathf.Abs(verticalAngle) > _verticalClamp;
        }

        return outOfHorizontal || outOfVertical;
    }
}
