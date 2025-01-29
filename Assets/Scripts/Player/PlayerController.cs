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

    // Input value
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

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<GroundedCharacterController>();
        _cameraController = GetComponent<CameraController>();
    }

    private void Update()
    {
        UpdateMoveSpeed();
        CheckJump();
        MoveAndRotate();
        UpdateAnimatorParameters();
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
            // 입력과 함께 점프 후 입력이 없어도 이동이 가능하고 그 이외에는 정지.
            if (!_isJumped)
            {
                _movement.Move(Vector3.zero);
            }
        }
    }

    private void CheckJump()
    {
        // 착지 상태로 판별을 안한 이유는 공중에서 착지 제한 시간이 끝난 뒤에
        // 착지 상태가 되기 때문에 점프한 후 바로 땅에 닿으면 착지 상태로 판별이 안됨.
        if (_movement.IsGrounded &&
            !_movement.IsJumping &&
            !_movement.IsFalling)
        {
            _isJumped = false;
        }

        if (_isPressedJump)
        {
            _isJumped = true;
            _isJumpedWithInput = _move != Vector2.zero;
            _isPressedJump = false;
            _movement.Jump(_jumpForce);
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

    private void UpdateAnimatorParameters()
    {
        var inputDirection = new Vector3(_move.x, 0f, _move.y);
        float targetSpeed = inputDirection == Vector3.zero ? 0f : _movement.MoveSpeed;
        float speedChangeRate = _movement.SpeedChangeRate * Time.deltaTime;

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
        _animator.SetFloat(_animIDPosX, 0f);
        _animator.SetFloat(_animIDPosY, 1f);
        _animator.SetBool(_animIDGrounded, _movement.IsGrounded);
        _animator.SetBool(_animIDJump, _movement.IsJumping);
        _animator.SetBool(_animIDFall, _movement.IsFalling);
        _animator.SetBool(_animIDLand, _movement.IsLanding);
    }

    private void RotateCamera()
    {
        _cameraController.Rotate(_look.y, _look.x);
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
        _isPressedJump = inputValue.isPressed;
    }
}
