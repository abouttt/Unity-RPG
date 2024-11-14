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
            TargetChanged?.Invoke(_target);
        }
    }

    public bool HasTarget => _target != null;
    public float ViewRadius => _viewRadius;
    public float ViewAngle => _viewAngle;

    [Header("Find")]
    [SerializeField]
    private float _viewRadius;

    [SerializeField, Range(0f, 360f)]
    private float _viewAngle;

    [SerializeField]
    private LayerMask _targetLayers;

    [SerializeField]
    private LayerMask _obstacleLayers;

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

        var targets = Physics.OverlapSphere(transform.position, _viewRadius, _targetLayers);
        foreach (var target in targets)
        {
            var directionToTarget = (target.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > _viewAngle * 0.5f)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            if (Physics.Linecast(transform.position, target.transform.position, _obstacleLayers))
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

        if (_distance && Vector3.Distance(transform.position, _target.position) > _viewRadius)
        {
            Target = null;
            return;
        }

        if (_obstacle && Physics.Linecast(transform.position, _target.position, _obstacleLayers))
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
