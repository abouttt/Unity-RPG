using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _inAirSpeed;

    [SerializeField]
    private float _landingSpeed;

    [SerializeField]
    private float _jumpForce;

    private bool _isJumped;
    private bool _isJumpedWithInput;

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;
    private bool _isPressedJump;

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
        CheckJump();
        UpdateMoveSpeed();
        Move();
        Rotate();
    }

    private void LateUpdate()
    {
        RotateCamera();
    }

    private void Move()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);

        if (inputDirection != Vector3.zero)
        {
            float cameraYaw = _mainCamera.transform.eulerAngles.y;
            float yaw = GetYaw(inputDirection) + cameraYaw;
            var movementDirection = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;

            _movement.Move(movementDirection);
        }
        else
        {
            if (!_isJumped)
            {
                _movement.Move(Vector3.zero);
            }
        }
    }

    private void Rotate()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);

        if (inputDirection != Vector3.zero)
        {
            float cameraYaw = _mainCamera.transform.eulerAngles.y;
            float yaw = GetYaw(inputDirection) + cameraYaw;
            var rotationDirection = new Vector3(0f, yaw, 0f);

            _movement.Rotate(rotationDirection);
        }
    }

    private void CheckJump()
    {
        if (_isJumped)
        {
            if (_movement.IsGrounded &&
                !_movement.IsJumping &&
                !_movement.IsFalling)
            {
                _isJumped = false;
            }

            _isPressedJump = false;
        }
        else
        {
            if (_isPressedJump)
            {
                _isJumped = true;
                _isJumpedWithInput = _move != Vector2.zero;
                _isPressedJump = false;
                _movement.Jump(_jumpForce);
            }
        }
    }

    private void UpdateMoveSpeed()
    {
        if (_movement.IsGrounded)
        {
            _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                                : _isPressedSprint ? _sprintSpeed
                                : _runSpeed;
        }
        else if (_isJumped)
        {
            if (!_isJumpedWithInput)
            {
                _movement.MoveSpeed = _inAirSpeed;
            }
        }
        else if (_movement.IsFalling)
        {
            _movement.MoveSpeed = _inAirSpeed;
        }
    }

    private void RotateCamera()
    {
        _cameraController.Rotate(_look.y, _look.x);
    }

    private float GetYaw(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    // Input System Callbacks

    public void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue inputValue)
    {
        _look = Managers.Input.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    public void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    public void OnJump(InputValue inputValue)
    {
        _isPressedJump = inputValue.isPressed;
    }
}
