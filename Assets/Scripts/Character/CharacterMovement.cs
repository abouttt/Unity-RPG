using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CharacterMovement : MonoBehaviour
{
    public bool IsGrounded => _isGrounded;
    public bool IsJumping => _isJumping;
    public bool IsFalling => _isFalling;
    public bool IsLanding => _isLanding;

    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }

    public float SpeedChangeRate
    {
        get => _speedChangeRate;
        set => _speedChangeRate = value;
    }

    public float JumpHeight
    {
        get => _jumpHeight;
        set => _jumpHeight = value;
    }

    public bool UseGravity
    {
        get => _useGravity;
        set => _useGravity = value;
    }

    [Space(10)]
    [SerializeField, ReadOnly]
    private bool _isGrounded;

    [SerializeField, ReadOnly]
    private bool _isJumping;

    [SerializeField, ReadOnly]
    private bool _isFalling;

    [SerializeField, ReadOnly]
    private bool _isLanding;

    [Header("Move")]
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _speedChangeRate;

    [SerializeField]
    private float _slopeRayLength;

    [SerializeField]
    private float _slopeVelocity;

    [Header("Rotation")]
    [SerializeField, Range(0f, 0.3f)]
    private float _rotationSmoothTime;

    [Header("Jump")]
    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private float _jumpTimeout;

    [SerializeField]
    private float _fallTimeout;

    [SerializeField]
    private float _landTimeout;

    [Header("Gravity")]
    [SerializeField]
    private bool _useGravity = true;

    [SerializeField, Min(1f)]
    private float _gravityMultiplier;

    [Header("Grounded")]
    [SerializeField]
    private float _groundedOffset = -0.14f;

    [SerializeField]
    private float _groundedRadius = 0.28f;

    [SerializeField]
    private LayerMask _groundLayers;

    private float _speed;
    private float _targetMove;
    private float _targetRotation;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private Vector3 _moveDirection;
    private Vector3 _targetDirection;
    private readonly float _gravity = -9.81f;
    private readonly float _terminalVelocity = 53f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _landTimeoutDelta;
    private bool _isGroundedTrigger;    // 공중에 떠있기 전 땅에있는 판단을 무시하기 위함
    private bool _canUpdateGroundedTimeouts = true;

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        CheckGrounded();
        UpdateTimeouts(deltaTime);
        ApplyGravity(deltaTime);
        ApplyMovement(deltaTime);
        ApplyRotation();
    }

    private void Start()
    {
        _targetRotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        _targetMove = _targetRotation;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    public void Move(Vector3 direction, float overrideYaw = 0f)
    {
        _moveDirection = direction;

        if (_moveDirection != Vector3.zero)
        {
            _targetMove = Mathf.Atan2(_moveDirection.x, _moveDirection.z) * Mathf.Rad2Deg + overrideYaw;
            _targetDirection = Quaternion.Euler(0f, _targetMove, 0f) * Vector3.forward;
        }
    }

    public void Rotate(Vector3 direction, float overrideYaw = 0f)
    {
        if (direction != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + overrideYaw;
        }
    }

    public void Jump()
    {
        if (_jumpTimeoutDelta > 0f)
        {
            return;
        }

        _isJumping = true;
        _isFalling = false;
        _isLanding = false;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
        _landTimeoutDelta = _landTimeout;
        _canUpdateGroundedTimeouts = false;
        _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity * _gravityMultiplier);
    }

    private void ApplyMovement(float deltaTime)
    {
        float targetSpeed = _moveDirection == Vector3.zero ? 0f : _moveSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // 목표 속도까지 가감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, _speedChangeRate * Time.deltaTime);
        }
        else
        {
            _speed = targetSpeed;
        }

        var move = _targetDirection.normalized * (_speed * deltaTime);
        var height = new Vector3(0f, _verticalVelocity * deltaTime, 0f);
        _controller.Move(move + height);
    }

    private void ApplyRotation()
    {
        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    private void ApplyGravity(float deltaTime)
    {
        if (!_useGravity)
        {
            return;
        }

        if (_isGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = OnSlope() ? _slopeVelocity : -2f;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * _gravityMultiplier * deltaTime;
        }
    }

    private bool OnSlope()
    {
        if (_isJumping)
        {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, _slopeRayLength))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }

        return false;
    }

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void UpdateTimeouts(float deltaTime)
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = _fallTimeout;

            if (_isGroundedTrigger)
            {
                _isJumping = false;
                _isFalling = false;
                _isLanding = true;
                _isGroundedTrigger = false;
                _canUpdateGroundedTimeouts = true;
            }

            if (_canUpdateGroundedTimeouts)
            {
                int timeoutEndedCount = 0;

                if (_jumpTimeoutDelta >= 0f)
                {
                    _jumpTimeoutDelta -= deltaTime;
                }
                else
                {
                    timeoutEndedCount++;
                }

                if (_landTimeoutDelta >= 0f)
                {
                    _landTimeoutDelta -= deltaTime;
                }
                else
                {
                    _isLanding = false;
                    timeoutEndedCount++;
                }

                if (timeoutEndedCount == 2)
                {
                    _canUpdateGroundedTimeouts = false;
                }
            }
        }
        else
        {
            _isGroundedTrigger = true;

            if (_fallTimeoutDelta >= 0f)
            {
                _fallTimeoutDelta -= deltaTime;
            }
            else
            {
                _isJumping = false;
                _isFalling = true;
                _landTimeoutDelta = _landTimeout;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        // CheckGrounded
        var transparentGreen = new Color(0f, 1f, 0f, 0.35f);
        var transparentRed = new Color(1f, 0f, 0f, 0.35f);

        if (_isGrounded)
        {
            Gizmos.color = transparentGreen;
        }
        else
        {
            Gizmos.color = transparentRed;
        }

        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, _groundedRadius);
#if UNITY_EDITOR
        Handles.Label(spherePosition, "Check Grounded");
#endif

        // Slope Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * _slopeRayLength);
#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.down * _slopeRayLength, "Slope Ray");
#endif
    }
}
