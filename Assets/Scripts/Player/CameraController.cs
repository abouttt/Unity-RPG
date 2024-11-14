using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform _cinemachineCameraTarget;

    [SerializeField]
    private float _sensitivity;

    [SerializeField]
    private float _topClamp;

    [SerializeField]
    private float _bottomClamp;

    private Quaternion _rotation;
    private Vector3 _eulerAngles;
    private readonly float _threshold = 0.01f;

    private void Start()
    {
        _rotation = _cinemachineCameraTarget.rotation;
        _eulerAngles = _rotation.eulerAngles;
    }

    public void Rotate(float pitch, float yaw)
    {
        if (new Vector2(pitch, yaw).sqrMagnitude >= _threshold)
        {
            _eulerAngles.x += pitch * _sensitivity;
            _eulerAngles.y += yaw * _sensitivity;
        }

        ApplyRotate();
    }

    public void LookRotate(Vector3 lookPoint, float speed)
    {
        var lookRotation = Quaternion.LookRotation(lookPoint - _cinemachineCameraTarget.position);
        _rotation = Quaternion.Slerp(_rotation, lookRotation, speed * Time.deltaTime);
        _eulerAngles = _rotation.eulerAngles;
        ApplyRotate();
    }

    private void ApplyRotate()
    {
        _eulerAngles.x = Util.ClampAngle(_eulerAngles.x, _bottomClamp, _topClamp);
        _eulerAngles.y = Util.ClampAngle(_eulerAngles.y, float.MinValue, float.MaxValue);
        _rotation = Quaternion.Euler(_eulerAngles.x, _eulerAngles.y, 0f);
        _cinemachineCameraTarget.rotation = _rotation;
    }
}
