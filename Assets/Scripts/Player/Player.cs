using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _runSpeed;

    [SerializeField]
    private float _sprintSpeed;

    [SerializeField]
    private float _lockOnRotationSpeed;

    private GameObject _mainCamera;
    private Animator _animator;
    private CharacterMovement _movement;
    private ThirdPersonCamera _camera;
    private LockOn _lockOn;

    // animation IDs
    private readonly int _animIDSpeed = Animator.StringToHash("Speed");
    private readonly int _animIDPosX = Animator.StringToHash("PosX");
    private readonly int _animIDPosY = Animator.StringToHash("PosY");

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<CharacterMovement>();
        _camera = GetComponent<ThirdPersonCamera>();
        _lockOn = GetComponent<LockOn>();

        _movement.MoveSpeed = _runSpeed;
    }

    private void Start()
    {
        Managers.Input.GetAction("Jump").performed += context => _movement.Jump();
        Managers.Input.GetAction("Sprint").started += context => _movement.MoveSpeed = _sprintSpeed;
        Managers.Input.GetAction("Sprint").canceled += context => _movement.MoveSpeed = _runSpeed;
        Managers.Input.GetAction("LockOn").performed += context =>
        {
            if (_lockOn.IsLockOn)
            {
                _lockOn.Target = null;
            }
            else
            {
                _lockOn.FindTarget(_mainCamera.transform);
            }
        };
    }

    private void Update()
    {
        _movement.Gravity();
        _movement.CheckGrounded();
        HandleMovement();
        UpdateAnimatorParameters();
    }

    private void LateUpdate()
    {
        if (_lockOn.IsLockOn)
        {
            _camera.LookRotate((_lockOn.Target.position + transform.position) * 0.5f, _lockOnRotationSpeed);
            _lockOn.TrackingTarget(_mainCamera.transform);
        }
        else
        {
            var look = Managers.Input.Look;
            _camera.Rotate(look.y, look.x);
        }
    }

    private void HandleMovement()
    {
        var move = Managers.Input.Move;
        var inputDirection = new Vector3(move.x, 0f, move.y).normalized;
        float cameraYaw = _mainCamera.transform.eulerAngles.y;

        _movement.Move(inputDirection, cameraYaw);

        if (_lockOn.IsLockOn && IsOnlyRun())
        {
            Vector3 rotationDirection = inputDirection == Vector3.zero
                ? Vector3.zero
                : (_lockOn.Target.position - transform.position).normalized;
            _movement.Rotate(rotationDirection);
        }
        else
        {
            _movement.Rotate(inputDirection, cameraYaw);
        }
    }

    private void UpdateAnimatorParameters()
    {
        bool isLockOnOnlyRun = _lockOn.IsLockOn && IsOnlyRun();
        _animator.SetFloat(_animIDSpeed, _movement.SpeedBlend);
        _animator.SetFloat(_animIDPosX, isLockOnOnlyRun ? _movement.PosXBlend : 0f);
        _animator.SetFloat(_animIDPosY, isLockOnOnlyRun ? _movement.PosYBlend : 1f);
    }

    private bool IsOnlyRun()
    {
        return !(Managers.Input.Sprint || _movement.IsJumping || _movement.IsFalling);
    }
}
