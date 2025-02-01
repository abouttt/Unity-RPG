using UnityEngine;

public class LockOnTargetTracker : MonoBehaviour
{
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

    public void TrackingLockOnTarget(FieldOfView fov)
    {
        if (!fov.HasTarget)
        {
            return;
        }

        if (IsTargetOutOfRange(fov) ||
            IsTargetObstructed(fov) ||
            IsTargetAngleInvalid(fov))
        {
            fov.Target = null;
        }
    }

    private bool IsTargetOutOfRange(FieldOfView fov)
    {
        return _distance && Vector3.Distance(fov.transform.position, fov.Target.position) > fov.ViewRadius;
    }

    private bool IsTargetObstructed(FieldOfView fov)
    {
        return _obstacle && Physics.Linecast(fov.transform.position, fov.Target.position, fov.ObstacleLayers);
    }

    private bool IsTargetAngleInvalid(FieldOfView fov)
    {
        return (_horizontalAngle && !IsAngleInRange(fov.transform.eulerAngles.y, -_horizontalAngleValue, _horizontalAngleValue)) ||
               (_verticalAngle && !IsAngleInRange(fov.transform.eulerAngles.x, -_verticalAngleValue, _verticalAngleValue));
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
