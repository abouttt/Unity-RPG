using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundedCharacterController : MonoBehaviour
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

    public float RotationSpeed
    {
        get => _rotationSpeed;
        set => _rotationSpeed = value;
    }

    public bool UseGravity
    {
        get => _useGravity;
        set => _useGravity = value;
    }

    [Header("     State")]
    [SerializeField, ReadOnly]
    private bool _isGrounded;

    [SerializeField, ReadOnly]
    private bool _isJumping;

    [SerializeField, ReadOnly]
    private bool _isFalling;

    [SerializeField, ReadOnly]
    private bool _isLanding;

    [Header("Movement")]
    [SerializeField]
    private float _moveSpeed;

    [SerializeField]
    private float _speedChangeRate;

    [Header("Rotation")]
    [SerializeField]
    private float _rotationSpeed;

    [Header("Slope")]
    [SerializeField]
    private float _slopeRayLength;

    [SerializeField]
    private float _slopeVelocity;

    [Header("Gravity")]
    [SerializeField]
    private bool _useGravity = true;

    [SerializeField]
    private float _gravity = -9.81f;

    [SerializeField, Min(1f)]
    private float _gravityMultiplier = 1f;

    [Header("Grounded")]
    [SerializeField]
    private float _groundedOffset = -0.14f;

    [SerializeField]
    private float _groundedRadius = 0.28f;

    [SerializeField]
    private LayerMask _groundLayers;

    [Header("Timeout")]
    [SerializeField]
    private float _jumpTimeout;

    [SerializeField]
    private float _fallTimeout;

    [SerializeField]
    private float _landTimeout;

    private Vector3 _movementDirection;
    private Vector3 _rotationDirection;
    private float _targetSpeed;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _landTimeoutDelta;
    private bool _groundedTrigger;  // 공중에 떠있기 전 땅에있는 판단을 무시하기 위함

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;

        CheckGrounded();
        UpdateState(deltaTime);
        ApplyGravity(deltaTime);
        ApplyMove(deltaTime);
        ApplyRotate(deltaTime);
    }

    public void Move(Vector3 direction, bool calcNormalize = true)
    {
        if (direction != Vector3.zero)
        {
            _movementDirection = calcNormalize ? direction.normalized : direction;
            _targetSpeed = _moveSpeed;
        }
        else
        {
            _targetSpeed = 0f;
        }
    }

    public void Rotate(Vector3 direction)
    {
        _rotationDirection = direction;
    }

    public void Jump(float force)
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
        _verticalVelocity = Mathf.Sqrt(force * -2f * _gravity * _gravityMultiplier);
    }

    private void ApplyMove(float deltaTime)
    {
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        float speed;

        // 목표 속도까지 가감속
        if (currentHorizontalSpeed < _targetSpeed - speedOffset ||
            currentHorizontalSpeed > _targetSpeed + speedOffset)
        {
            speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, _speedChangeRate * deltaTime);
        }
        else
        {
            speed = _targetSpeed;
        }

        var move = _movementDirection * (speed * deltaTime);
        var height = new Vector3(0f, _verticalVelocity * deltaTime, 0f);
        _controller.Move(move + height);
    }

    private void ApplyRotate(float deltaTime)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(_rotationDirection), _rotationSpeed * deltaTime);
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

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
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

    private void UpdateState(float deltaTime)
    {
        if (_isGrounded)
        {
            _fallTimeoutDelta = _fallTimeout;

            if (_groundedTrigger)
            {
                _isJumping = false;
                _isFalling = false;
                _isLanding = true;
                _groundedTrigger = false;
            }

            if (_jumpTimeoutDelta >= 0f)
            {
                _jumpTimeoutDelta -= deltaTime;
            }

            if (_landTimeoutDelta >= 0f)
            {
                _landTimeoutDelta -= deltaTime;
            }
            else
            {
                _isLanding = false;
            }
        }
        else
        {
            _groundedTrigger = true;

            if (_verticalVelocity < 0f)
            {
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

        // Slope Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * _slopeRayLength);
    }
}
