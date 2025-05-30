using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CameraController _cameraController;

    // Input variables
    private Vector2 _look;

    private void Awake()
    {
        _cameraController = GetComponent<CameraController>();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        _cameraController.Rotate(_look.y, _look.x);
    }

    // Input actions
    private void OnLook(InputValue inputValue)
    {
        _look = Managers.Input.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }
}
