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
            var directionToTarget = (target.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > ViewAngle * 0.5f)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            if (Physics.Linecast(transform.position, target.transform.position, ObstacleLayers))
            {
                continue;
            }

            finalTarget = target.transform;
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

        if (_distance && Vector3.Distance(transform.position, _target.position) > ViewRadius)
        {
            Target = null;
            return;
        }

        if (_obstacle && Physics.Linecast(transform.position, _target.position, ObstacleLayers))
        {
            Target = null;
            return;
        }

        if (_horizontalAngle &&
            !IsAngleInRange(transform.eulerAngles.y, -_horizontalAngleValue, _horizontalAngleValue))
        {
            Target = null;
            return;
        }

        if (_verticalAngle &&
            !IsAngleInRange(transform.eulerAngles.x, -_verticalAngleValue, _verticalAngleValue))
        {
            Target = null;
        }
    }

    private bool IsAngleInRange(float angle, float min, float max)
    {
        angle = Util.ClampAngle(angle, min, max);
        return angle >= min && angle <= max;
    }
}
