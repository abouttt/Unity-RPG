using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(CharacterController))]
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

    private void Start()
    {
        _targetRotation = Mathf.Atan2(transform.forward.x, transform.forward.z) * Mathf.Rad2Deg;
        _targetMove = _targetRotation;
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }

    private void Update()
    {
        CheckGrounded();
        UpdateTimeouts();
        ApplyGravity();
        ApplyMovement();
    }

    public void Move(Vector3 direction, float overrideYaw = 0f)
    {
        bool isZeroDirection = direction == Vector3.zero;
        float targetSpeed = isZeroDirection ? 0f : _moveSpeed;
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;

        // 목표 속도까지 가감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, _speedChangeRate * Time.deltaTime);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        if (!isZeroDirection)
        {
            _targetMove = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + overrideYaw;
            _targetDirection = Quaternion.Euler(0f, _targetMove, 0f) * Vector3.forward;
        }
    }

    public void Rotate(Vector3 direction, float overrideYaw = 0f)
    {
        if (direction != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + overrideYaw;
        }

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
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

    private void UpdateTimeouts()
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
                if (_jumpTimeoutDelta >= 0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }

                if (_landTimeoutDelta >= 0f)
                {
                    _landTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    _isLanding = false;
                }
            }
        }
        else
        {
            _isGroundedTrigger = true;

            if (_fallTimeoutDelta >= 0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _isJumping = false;
                _isFalling = true;
                _landTimeoutDelta = _landTimeout;
            }
        }
    }

    private void ApplyGravity()
    {
        if (_useGravity)
        {
            if (_isGrounded && _verticalVelocity < 0f)
            {
                if (OnSlope())
                {
                    _verticalVelocity = _slopeVelocity;
                }
                else
                {
                    _verticalVelocity = -2f;
                }
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += _gravity * _gravityMultiplier * Time.deltaTime;
            }
        }
        else
        {
            _verticalVelocity = 0f;
        }
    }

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void ApplyMovement()
    {
        var move = _targetDirection.normalized * (_speed * Time.deltaTime);
        var height = new Vector3(0f, _verticalVelocity * Time.deltaTime, 0f);
        _controller.Move(move + height);
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

    private void OnDrawGizmosSelected()
    {
        #region CheckGrounded
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
        #endregion

        #region Slope Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * _slopeRayLength);
#if UNITY_EDITOR
        Handles.Label(transform.position + Vector3.down * _slopeRayLength, "Slope Ray");
#endif
        #endregion
    }
}
