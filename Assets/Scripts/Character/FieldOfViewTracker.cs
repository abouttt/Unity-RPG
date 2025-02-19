using UnityEngine;

public class FieldOfViewTracker : MonoBehaviour
{
    [field: SerializeField]
    public FieldOfView FieldOfView { get; set; }

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

    private void LateUpdate()
    {
        TrackingTarget();
    }

    public void TrackingTarget()
    {
        if (FieldOfView == null)
        {
            return;
        }

        if (!FieldOfView.HasTarget)
        {
            return;
        }

        if (IsTargetOutOfRange() ||
            IsTargetObstructed() ||
            IsTargetAngleInvalid())
        {
            FieldOfView.Target = null;
        }
    }

    private bool IsTargetOutOfRange()
    {
        return _distance && Vector3.Distance(FieldOfView.transform.position, FieldOfView.Target.position) > FieldOfView.ViewRadius;
    }

    private bool IsTargetObstructed()
    {
        return _obstacle && Physics.Linecast(FieldOfView.transform.position, FieldOfView.Target.position, FieldOfView.ObstacleLayers);
    }

    private bool IsTargetAngleInvalid()
    {
        return (_horizontal && !IsAngleInRange(FieldOfView.transform.eulerAngles.y, -_horizontalClamp, _horizontalClamp)) ||
               (_vertical && !IsAngleInRange(FieldOfView.transform.eulerAngles.x, -_verticalClamp, _verticalClamp));
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
