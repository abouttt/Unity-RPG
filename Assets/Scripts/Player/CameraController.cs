using UnityEngine;

public class CameraController : MonoBehaviour
{
    [field: Header("Settings")]
    [field: SerializeField]
    public float Sensitivity { get; set; } = 1f;

    [field: SerializeField]
    public float SensitivityMultiplier { get; set; } = 0.5f;

    [SerializeField]
    private float _smoothingSpeed = 10f;

    [SerializeField]
    private float _smoothingTime = 0.1f;

    [field: Header("Options")]
    [field: SerializeField]
    public bool InvertX { get; set; }

    [field: SerializeField]
    public bool InvertY { get; set; }

    [field: SerializeField]
    public bool UseLerp { get; set; } = true;

    [field: Header("Clamping")]
    [field: SerializeField]
    public float TopClamp { get; set; } = 90f;

    [field: SerializeField]
    public float BottomClamp { get; set; } = -90f;

    [field: Header("References")]
    [field: SerializeField]
    public Transform CameraTarget { get; set; }

    private float _targetPitch;
    private float _targetYaw;
    private float _currentPitch;
    private float _currentYaw;
    private float _pitchVelocity;
    private float _yawVelocity;

    private void Start()
    {
        var angles = CameraTarget.eulerAngles;
        _targetPitch = _currentPitch = Utility.WrapAngle(angles.x);
        _targetYaw = _currentYaw = angles.y;
    }

    private void LateUpdate()
    {
        if (CameraTarget == null)
        {
            return;
        }

        UpdateRotation();
    }

    public void Rotate(Vector2 input)
    {
        if (input.IsNearlyZero())
        {
            return;
        }

        float sensitivity = Sensitivity * SensitivityMultiplier;
        _targetPitch += (InvertY ? -input.y : input.y) * sensitivity;
        _targetYaw += (InvertX ? -input.x : input.x) * sensitivity;
    }

    public void LookAt(Vector3 targetPoint)
    {
        if (CameraTarget == null)
        {
            return;
        }

        var direction = targetPoint - CameraTarget.position;
        if (direction.IsNearlyZero())
        {
            return;
        }

        var targetRot = Quaternion.LookRotation(direction);
        var euler = targetRot.eulerAngles;

        _targetPitch = Utility.WrapAngle(euler.x);
        _targetYaw = euler.y;
    }

    private void UpdateRotation()
    {
        _targetPitch = Mathf.Clamp(_targetPitch, BottomClamp, TopClamp);

        if (UseLerp)
        {
            _currentPitch = Mathf.LerpAngle(_currentPitch, _targetPitch, _smoothingSpeed * Time.deltaTime);
            _currentYaw = Mathf.LerpAngle(_currentYaw, _targetYaw, _smoothingSpeed * Time.deltaTime);
        }
        else
        {
            _currentPitch = Mathf.SmoothDampAngle(_currentPitch, _targetPitch, ref _pitchVelocity, _smoothingTime);
            _currentYaw = Mathf.SmoothDampAngle(_currentYaw, _targetYaw, ref _yawVelocity, _smoothingTime);
        }

        CameraTarget.rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
    }
}
