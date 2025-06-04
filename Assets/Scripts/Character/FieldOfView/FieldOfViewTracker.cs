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
        Transform origin = _fieldOfView.transform;
        Vector3 toTarget = (_fieldOfView.Target.position - origin.position).normalized;

        bool outOfHorizontal = false;
        if (_horizontal)
        {
            Vector3 flatForward = new Vector3(origin.forward.x, 0f, origin.forward.z).normalized;
            Vector3 flatToTarget = new Vector3(toTarget.x, 0f, toTarget.z).normalized;

            float horizontalAngle = Vector3.Angle(flatForward, flatToTarget);
            outOfHorizontal = horizontalAngle > _horizontalClamp;
        }

        bool outOfVertical = false;
        if (_vertical)
        {
            float verticalAngle = Vector3.Angle(toTarget, new Vector3(toTarget.x, 0f, toTarget.z));
            outOfVertical = verticalAngle > _verticalClamp;
        }

        return outOfHorizontal || outOfVertical;
    }
}
