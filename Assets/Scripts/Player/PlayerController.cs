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

    private GameObject _mainCamera;
    private Animator _animator;
    private GroundedCharacterController _movement;
    private CameraController _camera;

    private bool _isJumped;
    private bool _isJumpedWithInput;

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;
    private bool _isPressedJump;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<GroundedCharacterController>();
        _camera = GetComponent<CameraController>();
    }

    private void Update()
    {
        CheckJump();
        UpdateMoveSpeed();
        Move();
        Rotate();
        RotateCamera();
    }

    private void Move()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);
        if (inputDirection != Vector3.zero)
        {
            float yaw = GetYaw(inputDirection) + _mainCamera.transform.eulerAngles.y;
            var direction = Quaternion.Euler(0f, yaw, 0f) * Vector3.forward;
            _movement.Move(direction);
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
            float yaw = GetYaw(inputDirection) + _mainCamera.transform.eulerAngles.y;
            var direction = new Vector3(0f, yaw, 0f);
            _movement.Rotate(direction);
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
                _movement.Jump();
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
        _camera.Rotate(_look);
    }

    private float GetYaw(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    // Input actions

    private void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    private void OnLook(InputValue inputValue)
    {
        _look = Managers.Input.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    private void OnJump(InputValue inputValue)
    {
        _isPressedJump = inputValue.isPressed;
    }
}
