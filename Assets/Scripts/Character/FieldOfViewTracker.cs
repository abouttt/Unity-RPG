using UnityEngine;

public class FieldOfViewTracker : MonoBehaviour
{
    [field: SerializeField]
    public FieldOfView FOV { get; set; }

    [SerializeField]
    private bool _distance = true;

    [SerializeField]
    private bool _obstacle = true;

    [SerializeField]
    private bool _horizontalAngle = true;

    [SerializeField]
    private bool _verticalAngle = true;

    [SerializeField]
    private float _horizontalAngleValue;

    [SerializeField]
    private float _verticalAngleValue;

    private void LateUpdate()
    {
        TrackingTarget();
    }

    public void TrackingTarget()
    {
        if (!FOV.HasTarget)
        {
            return;
        }

        if (IsTargetOutOfRange() ||
            IsTargetObstructed() ||
            IsTargetAngleInvalid())
        {
            FOV.Target = null;
        }
    }

    private bool IsTargetOutOfRange()
    {
        return _distance && Vector3.Distance(FOV.transform.position, FOV.Target.position) > FOV.ViewRadius;
    }

    private bool IsTargetObstructed()
    {
        return _obstacle && Physics.Linecast(FOV.transform.position, FOV.Target.position, FOV.ObstacleLayers);
    }

    private bool IsTargetAngleInvalid()
    {
        return (_horizontalAngle && !IsAngleInRange(FOV.transform.eulerAngles.y, -_horizontalAngleValue, _horizontalAngleValue)) ||
               (_verticalAngle && !IsAngleInRange(FOV.transform.eulerAngles.x, -_verticalAngleValue, _verticalAngleValue));
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
