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
    private float _lockOnRotationSpeed;

    // 애니메이션 블렌드
    private float _speedBlend;
    private float _posXBlend;
    private float _posYBlend;

    private GameObject _mainCamera;
    private Animator _animator;
    private CharacterMovement _movement;
    private CameraController _cameraController;
    private FieldOfView _lockOnFov;
    private Interactor _interactor;

    // 애니메이션 아이디
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosY = Animator.StringToHash("PosY");
    private readonly int _animIDGrounded = Animator.StringToHash("Grounded");
    private readonly int _animIDJump = Animator.StringToHash("Jump");
    private readonly int _animIDFall = Animator.StringToHash("Fall");

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<CharacterMovement>();
        _cameraController = GetComponent<CameraController>();
        _lockOnFov = _mainCamera.GetComponent<FieldOfView>();
        _interactor = GetComponentInChildren<Interactor>();
    }

    private void Update()
    {
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
        float cameraYaw = _mainCamera.transform.eulerAngles.y;

        if (_movement.IsGrounded)
        {
            _movement.MoveSpeed = _movement.IsLanding ? _landingSpeed
                                : _isPressedSprint ? _sprintSpeed
                                : _runSpeed;
        }

        _movement.Move(inputDirection, cameraYaw);

        if (_lockOnFov.HasTarget && IsOnlyRun())
        {
            var rotationDirection = inputDirection == Vector3.zero
                                  ? Vector3.zero
                                  : (_lockOnFov.Target.position - transform.position).normalized;

            _movement.Rotate(rotationDirection);
        }
        else
        {
            _movement.Rotate(inputDirection, cameraYaw);
        }
    }

    private void RotateCamera()
    {
        if (_lockOnFov.HasTarget)
        {
            var lookPosition = (_lockOnFov.Target.position + transform.position) * 0.5f;
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
    }

    private bool IsOnlyRun()
    {
        return !(_isPressedSprint || _movement.IsJumping || _movement.IsFalling || _movement.IsLanding);
    }

    // Input

    private void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    private void OnLook(InputValue inputValue)
    {
        _look = InputManager.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    private void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    private void OnJump(InputValue inputValue)
    {
        _movement.Jump();
    }

    private void OnLockOn(InputValue inputValue)
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

    private void OnInteract(InputValue inputValue)
    {
        _interactor.Interact = inputValue.isPressed;
    }
}
