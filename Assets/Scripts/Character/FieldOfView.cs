using System;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    public event Action<Transform> TargetChanged;

    public Transform Target
    {
        get => _target;
        set
        {
            if (_target == value)
            {
                return;
            }

            _target = value;
            HasTarget = _target != null;
            TargetChanged?.Invoke(_target);
        }
    }

    public bool HasTarget { get; private set; }

    [field: Header("Find")]
    [field: SerializeField]
    public float ViewRadius { get; set; }

    [field: SerializeField, Range(0f, 360f)]
    public float ViewAngle { get; set; }

    [field: SerializeField]
    public LayerMask TargetLayers { get; set; }

    [field: SerializeField]
    public LayerMask ObstacleLayers { get; set; }

    [Header("Tracking")]
    [SerializeField]
    private bool _distance;

    [SerializeField]
    private bool _obstacle;

    [SerializeField]
    private bool _horizontalAngle = true;

    [SerializeField]
    private bool _verticalAngle = true;

    [SerializeField]
    private float _horizontalAngleValue;

    [SerializeField]
    private float _verticalAngleValue;

    private Transform _target;

    private void LateUpdate()
    {
        TrackingTarget();
    }

    public void FindTarget()
    {
        Transform finalTarget = null;
        float shortestAngle = Mathf.Infinity;

        var targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetLayers);
        foreach (var target in targets)
        {
            var targetTransform = target.transform;
            var directionToTarget = (targetTransform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > ViewAngle * 0.5f)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            if (IsTargetObstructed(targetTransform))
            {
                continue;
            }

            finalTarget = targetTransform;
            shortestAngle = angle;
        }

        Target = finalTarget;
    }

    private void TrackingTarget()
    {
        if (_target == null)
        {
            return;
        }

        if (IsTargetOutOfRange(_target) || 
            IsTargetObstructed(_target) || 
            IsTargetAngleInvalid())
        {
            Target = null;
        }
    }

    private bool IsTargetObstructed(Transform target)
    {
        return Physics.Linecast(transform.position, target.position, ObstacleLayers);
    }

    private bool IsTargetOutOfRange(Transform target)
    {
        return Vector3.Distance(transform.position, target.position) > ViewRadius;
    }

    private bool IsTargetAngleInvalid()
    {
        return (_horizontalAngle && !IsAngleInRange(transform.eulerAngles.y, -_horizontalAngleValue, _horizontalAngleValue)) ||
               (_verticalAngle && !IsAngleInRange(transform.eulerAngles.x, -_verticalAngleValue, _verticalAngleValue));
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
