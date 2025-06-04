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
            TargetChanged?.Invoke(_target);
        }
    }

    [field: SerializeField]
    public float ViewRadius { get; set; }

    [field: SerializeField, Range(0f, 360f)]
    public float ViewAngle { get; set; }

    [field: SerializeField]
    public LayerMask TargetLayer { get; set; }

    [field: SerializeField]
    public LayerMask ObstacleLayer { get; set; }

    public bool HasTarget => _target != null;

    private Transform _target;

    public void FindTarget()
    {
        Transform finalTarget = null;
        var position = transform.position;
        float shortestAngle = Mathf.Infinity;

        var targets = Physics.OverlapSphere(transform.position, ViewRadius, TargetLayer);
        foreach (var target in targets)
        {
            var targetPosition = target.transform.position;
            var direction = (targetPosition - position).normalized;
            var angle = Vector3.Angle(transform.forward, direction);

            if (angle > ViewAngle / 2f)
            {
                continue;
            }

            if (angle >= shortestAngle)
            {
                continue;
            }

            var distance = Vector3.Distance(position, targetPosition);
            if (Physics.Raycast(position, direction, distance, ObstacleLayer))
            {
                continue;
            }

            finalTarget = target.transform;
            shortestAngle = angle;
        }

        Target = finalTarget;
    }
}
