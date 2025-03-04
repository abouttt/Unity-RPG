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

        if (IsTargetOutOfRange() ||
            IsTargetObstructed() ||
            IsTargetAngleInvalid())
        {
            _fieldOfView.Target = null;
        }
    }

    private bool IsTargetOutOfRange()
    {
        return _distance && Vector3.Distance(_fieldOfView.transform.position, _fieldOfView.Target.position) > _fieldOfView.ViewRadius;
    }

    private bool IsTargetObstructed()
    {
        return _obstacle && Physics.Linecast(_fieldOfView.transform.position, _fieldOfView.Target.position, _fieldOfView.ObstacleLayers);
    }

    private bool IsTargetAngleInvalid()
    {
        return (_horizontal && !IsAngleInRange(_fieldOfView.transform.eulerAngles.y, -_horizontalClamp, _horizontalClamp)) ||
               (_vertical && !IsAngleInRange(_fieldOfView.transform.eulerAngles.x, -_verticalClamp, _verticalClamp));
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
