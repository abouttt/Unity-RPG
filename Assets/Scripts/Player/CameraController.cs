using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field: SerializeField]
    public float Sensitivity { get; set; } = 1f;

    [SerializeField]
    private float _sensitivityMultiplier = 10f;

    [SerializeField]
    private Transform _cameraTarget;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    private float _pitch;
    private float _yaw;
    private readonly float _threshold = 0.01f;

    private void Start()
    {
        var angles = _cameraTarget.rotation.eulerAngles;
        _pitch = angles.x;
        _yaw = angles.y;
    }

    public void Rotate(float pitch, float yaw)
    {
        if (Mathf.Abs(pitch) >= _threshold || Mathf.Abs(yaw) >= _threshold)
        {
            var targetSensitivity = Sensitivity * _sensitivityMultiplier * Time.deltaTime;
            _pitch -= pitch * targetSensitivity;
            _yaw += yaw * targetSensitivity;
        }

        ApplyRotate();
    }

    public void LookRotate(Vector3 lookPoint, float speed)
    {
        var direction = lookPoint - _cameraTarget.position;
        if (direction.sqrMagnitude < 0.0001f)
        {
            return;
        }

        var lookRotation = Quaternion.LookRotation(direction.normalized);
        var smoothed = Quaternion.Slerp(_cameraTarget.rotation, lookRotation, speed * Time.deltaTime);
        var angles = smoothed.eulerAngles;
        _pitch = angles.x;
        _yaw = angles.y;

        ApplyRotate();
    }

    private void ApplyRotate()
    {
        _pitch = Util.ClampAngle(_pitch, _bottomClamp, _topClamp);
        _yaw = Util.ClampAngle(_yaw, float.MinValue, float.MaxValue);
        _cameraTarget.rotation = Quaternion.Euler(_pitch, _yaw, 0f);
    }
}
