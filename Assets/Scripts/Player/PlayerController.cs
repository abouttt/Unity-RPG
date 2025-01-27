using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _landingSpeed;

    [SerializeField]
    private float _jumpForce;

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;

    private GameObject _mainCamera;
    private GroundedCharacterController _movement;
    private CameraController _cameraController;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _movement = GetComponent<GroundedCharacterController>();
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        UpdateMoveSpeed();
        MoveAndRotate();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void MoveAndRotate()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);

        if (inputDirection != Vector3.zero)
        {
            float cameraYaw = _mainCamera.transform.eulerAngles.y;
            float y = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + cameraYaw;
            var movementDirection = Quaternion.Euler(0f, y, 0f) * Vector3.forward;
            var rotationDirection = new Vector3(0f, y, 0f);

            _movement.Move(movementDirection);
            _movement.Rotate(rotationDirection);
        }
        else
        {
            _movement.Move(Vector3.zero);
        }
    }

    private void RotateCamera()
    {
        _cameraController.Rotate(_look.y, _look.x);
    }

    private void UpdateMoveSpeed()
    {
        if (_movement.IsGrounded)
        {
            _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                                  : _isPressedSprint ? _sprintSpeed
                                  : _runSpeed;
        }
    }

    // Input System Callbacks

    public void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue inputValue)
    {
        _look = InputManager.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    public void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    public void OnJump(InputValue inputValue)
    {
        _movement.Jump(_jumpForce);
    }
}
