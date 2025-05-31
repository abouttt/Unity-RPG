using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundedCharacterController : MonoBehaviour
{
    [field: Header("State")]
    [field: SerializeField, ReadOnly]
    public bool IsGrounded { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsJumping { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsFalling { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsLanding { get; private set; }

    [field: Header("Movement")]
    [field: SerializeField]
    public float MoveSpeed { get; set; }

    [field: SerializeField]
    public float Acceleration { get; set; }

    [field: SerializeField]
    public float Deceleration { get; set; }

    [field: Header("Rotation")]
    [field: SerializeField]
    public float RotationSpeed { get; set; }

    [field: Header("Jump & Gravity")]
    [field: SerializeField]
    public float JumpHeight { get; set; }

    [field: SerializeField]
    public bool UseGravity { get; set; } = true;

    [field: SerializeField]
    public float Gravity { get; set; } = -9.81f;

    [field: SerializeField]
    public float GravityMultiplier { get; set; } = 2f;

    [field: Header("Slope")]
    [field: SerializeField]
    public float SlopeRayLength { get; set; }

    [field: SerializeField]
    public float SlopeVelocity { get; set; }

    [field: Header("Ground Check")]
    [field: SerializeField]
    public float GroundCheckOffset { get; set; }

    [field: SerializeField]
    public float GroundCheckRadius { get; set; }

    [field: SerializeField]
    public LayerMask GroundLayer { get; set; }

    [field: Header("Timeouts")]
    [field: SerializeField]
    public float JumpTimeout { get; set; }

    [field: SerializeField]
    public float FallTimeout { get; set; }

    [field: SerializeField]
    public float LandTimeout { get; set; }

    private CharacterController _controller;

    private Vector3 _moveDirection;
    private Vector3 _rotateDirection;

    private Vector3 _currentVelocity;
    private Vector3 _horizontalVelocity;
    private float _verticalVelocity;
    private readonly float _terminalVelocity = 53f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _landTimeoutDelta;
    private bool _groundedTrigger;  // 공중에 떠있기 전 땅에있는 판단을 무시하기 위함

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        float deltaTime = Time.deltaTime;
        CheckGrounded();
        UpdateTimeouts(deltaTime);
        UpdateGravity(deltaTime);
        UpdateMove(deltaTime);
        UpdateRotate(deltaTime);
    }

    public void Move(Vector3 direction)
    {
        _moveDirection = direction;
    }

    public void Rotate(Vector3 direction)
    {
        _rotateDirection = direction;
    }

    public void Jump()
    {
        if (_jumpTimeoutDelta > 0f)
        {
            return;
        }

        IsJumping = true;
        IsFalling = false;
        IsLanding = false;
        _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity * GravityMultiplier);
    }

    private void UpdateMove(float deltaTime)
    {
        float targetSpeed;
        float accelerationRate;
        if (_moveDirection.sqrMagnitude > 0.01f)
        {
            targetSpeed = MoveSpeed;
            accelerationRate = Acceleration;
        }
        else
        {
            targetSpeed = 0f;
            accelerationRate = Deceleration;
        }

        _horizontalVelocity = Vector3.Lerp(
            new Vector3(_currentVelocity.x, 0f, _currentVelocity.z),
            _moveDirection * targetSpeed,
            accelerationRate * deltaTime
        );

        _currentVelocity = new Vector3(_horizontalVelocity.x, _verticalVelocity, _horizontalVelocity.z);
        _controller.Move(_currentVelocity * deltaTime);
    }

    private void UpdateRotate(float deltaTime)
    {
        var targetRotation = Quaternion.Euler(_rotateDirection);
        var rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * deltaTime);
        transform.rotation = rotation;
    }

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundCheckOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundCheckRadius, GroundLayer, QueryTriggerInteraction.Ignore);
    }

    private void UpdateGravity(float deltaTime)
    {
        if (!UseGravity)
        {
            _verticalVelocity = 0f;
            return;
        }

        if (IsGrounded && _verticalVelocity < 0f)
        {
            _verticalVelocity = IsOnSlope() ? SlopeVelocity : -2f;
        }

        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += Gravity * GravityMultiplier * deltaTime;
        }
    }

    private void UpdateTimeouts(float deltaTime)
    {
        if (IsGrounded)
        {
            _fallTimeoutDelta = FallTimeout;

            if (_groundedTrigger)
            {
                IsJumping = false;
                IsFalling = false;
                IsLanding = true;
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
                IsLanding = false;
            }
        }
        else
        {
            _jumpTimeoutDelta = JumpTimeout;
            _groundedTrigger = true;

            // 점프 최대 높이에서 추락하는 지점
            if (_verticalVelocity < 0f)
            {
                if (_fallTimeoutDelta >= 0f)
                {
                    _fallTimeoutDelta -= deltaTime;
                }
                else
                {
                    IsJumping = false;
                    IsFalling = true;
                    _landTimeoutDelta = LandTimeout;
                }
            }
        }
    }

    private bool IsOnSlope()
    {
        if (IsJumping)
        {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, SlopeRayLength, GroundLayer))
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
        // CheckGrounded
        var transparentGreen = new Color(0f, 1f, 0f, 0.35f);
        var transparentRed = new Color(1f, 0f, 0f, 0.35f);
        Gizmos.color = IsGrounded ? transparentGreen : transparentRed;

        var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundCheckOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, GroundCheckRadius);

        // Slope Ray
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, Vector3.down * SlopeRayLength);
    }
}
