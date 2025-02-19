using UnityEngine;

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
    private PlayerInputValue _input;
    private Animator _animator;
    private GroundedCharacterController _movement;
    private CameraController _cameraController;
    private FieldOfView _lockOnFov;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _input = GetComponent<PlayerInputValue>();
        _animator = GetComponentInChildren<Animator>();
        _movement = GetComponent<GroundedCharacterController>();
        _cameraController = GetComponent<CameraController>();
        _lockOnFov = _mainCamera.GetComponent<FieldOfView>();
    }

    private void Update()
    {
        CheckJump();
        CheckLockOn();
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
        var inputDirection = new Vector3(_input.Move.x, 0f, _input.Move.y);

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
        var inputDirection = new Vector3(_input.Move.x, 0f, _input.Move.y);

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

            _input.Jump = false;
        }
        else
        {
            if (_input.Jump)
            {
                _isJumped = true;
                _isJumpedWithInput = _input.Move != Vector2.zero;
                _input.Jump = false;
                _movement.Jump(_jumpForce);
            }
        }
    }

    private void CheckLockOn()
    {
        if (!_input.LockOn)
        {
            return;
        }

        if (_lockOnFov.HasTarget)
        {
            _lockOnFov.Target = null;
        }
        else
        {
            _lockOnFov.FindTarget();
        }

        _input.LockOn = false;
    }

    private void UpdateMoveSpeed()
    {
        if (_movement.IsGrounded)
        {
            _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                                : _input.Sprint ? _sprintSpeed
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
            _cameraController.Rotate(_input.Look.y, _input.Look.x);
        }
    }

    private void UpdateAnimatorParameters()
    {
        var inputDirection = new Vector3(_input.Move.x, 0f, _input.Move.y);
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

    private bool IsOnlyRun()
    {
        return !(_input.Sprint || _movement.IsJumping || _movement.IsFalling || _movement.IsLanding);
    }

    private float GetYaw(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
