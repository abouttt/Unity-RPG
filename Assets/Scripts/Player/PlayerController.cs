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

    [SerializeField]
    private float _lockOnRotationSpeed;

    private bool _isJumped;
    private bool _isJumpedWithInput;

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;
    private bool _isPressedJump;

    // Animation blend
    private float _speedBlend;
    private float _posXBlend;
    private float _posYBlend;

    // Animation ID
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosY = Animator.StringToHash("PosY");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFall = Animator.StringToHash("Fall");
    private readonly int _animIDLand = Animator.StringToHash("Land");

    private GameObject _mainCamera;
    private Animator _animator;
    private GroundedCharacterController _movement;
    private CameraController _cameraController;
    private FieldOfView _lockOnFov;
    private Interactor _interactor;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<GroundedCharacterController>();
        _cameraController = GetComponent<CameraController>();
        _lockOnFov = _mainCamera.GetComponent<FieldOfView>();
        _interactor = GetComponent<Interactor>();
    }

    private void Update()
    {
        CheckJump();
        UpdateMoveSpeed();
        Move();
        Rotate();
        UpdateAnimatorParameters();
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
            float yaw;

            if (_lockOnFov.HasTarget && IsOnlyRun())
            {
                var directionToTarget = (_lockOnFov.Target.position - transform.position).normalized;
                yaw = GetYaw(directionToTarget);
            }
            else
            {
                float cameraYaw = _mainCamera.transform.eulerAngles.y;
                yaw = GetYaw(inputDirection) + cameraYaw;
            }

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
        if (_lockOnFov.HasTarget)
        {
            var lookPosition = (_lockOnFov.Target.position + transform.position) / 2;
            _cameraController.LookRotate(lookPosition, _lockOnRotationSpeed);
        }
        else
        {
            _cameraController.Rotate(_look.y, _look.x);
        }
    }

    private void UpdateAnimatorParameters()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);
        float targetSpeed = inputDirection == Vector3.zero ? 0f : _movement.MoveSpeed;
        float speedChangeRate = _movement.SpeedChangeRate * Time.deltaTime;
        bool isLockOnOnlyRun = _lockOnFov.HasTarget && IsOnlyRun();

        _speedBlend = Mathf.Lerp(_speedBlend, targetSpeed, speedChangeRate);
        _posXBlend = Mathf.Lerp(_posXBlend, inputDirection.x, speedChangeRate);
        _posYBlend = Mathf.Lerp(_posYBlend, inputDirection.z, speedChangeRate);

        if (_speedBlend < 0.01f)
        {
            _speedBlend = 0f;
            _posXBlend = 0f;
            _posYBlend = 0f;
        }

        _animator.SetFloat(_animIDSpeed, _speedBlend);
        _animator.SetFloat(_animIDPosX, isLockOnOnlyRun ? _posXBlend : 0f);
        _animator.SetFloat(_animIDPosY, isLockOnOnlyRun ? _posYBlend : 1f);
        _animator.SetBool(_animIDGrounded, _movement.IsGrounded);
        _animator.SetBool(_animIDJump, _movement.IsJumping);
        _animator.SetBool(_animIDFall, _movement.IsFalling);
        _animator.SetBool(_animIDLand, _movement.IsLanding);
    }

    private float GetYaw(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }

    private bool IsOnlyRun()
    {
        return !(_isPressedSprint || _movement.IsJumping || _movement.IsFalling || _movement.IsLanding);
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

    public void OnLockOn(InputValue inputValue)
    {
        if (_lockOnFov.HasTarget)
        {
            _lockOnFov.Target = null;
        }
        else
        {
            _lockOnFov.FindTarget();
        }
    }

    public void OnInteract(InputValue inputValue)
    {
        _interactor.Interact = inputValue.isPressed;
    }
}
