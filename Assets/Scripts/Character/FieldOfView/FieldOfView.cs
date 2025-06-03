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

    [field: SerializeField]
    public float ViewRadius { get; set; }

    [field: SerializeField, Range(0f, 360f)]
    public float ViewAngle { get; set; }

    [field: SerializeField]
    public LayerMask TargetLayer { get; set; }

    [field: SerializeField]
    public LayerMask ObstacleLayer { get; set; }

    private Transform _target;

    public void FindTarget()
    {
        Transform finalTarget = null;
        float shortestAngle = Mathf.Infinity;

        var targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetLayer);
        foreach (var target in targets)
        {
            var directionToTarget = (target.transform.position - transform.position).normalized;
            var angle = Vector3.Angle(transform.forward, directionToTarget);

            if (angle > ViewAngle / 2)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            var distanceToTarget = Vector3.Distance(transform.position, target.transform.position);
            if (Physics.Raycast(transform.position, directionToTarget, distanceToTarget, ObstacleLayer))
            {
                continue;
            }

            finalTarget = target.transform;
            shortestAngle = angle;
        }

        Target = finalTarget;
    }
}
