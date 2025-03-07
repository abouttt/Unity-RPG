using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class GroundedCharacterController : MonoBehaviour
{
    [field: Header("     State")]
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
    public float SpeedChangeRate { get; set; }

    [field: Header("Rotation")]
    [field: SerializeField]
    public float RotationSpeed { get; set; }

    [field: Header("Slope")]
    [field: SerializeField]
    public float SlopeRayLength { get; set; }

    [field: SerializeField]
    public float SlopeVelocity { get; set; }

    [field: Header("Gravity")]
    [field: SerializeField]
    public bool UseGravity { get; set; } = true;

    [field: SerializeField]
    public float Gravity { get; set; } = -9.81f;

    [field: SerializeField]
    public float GravityMultiplier { get; set; } = 1f;

    [field: Header("Grounded")]
    [field: SerializeField]
    public float GroundedOffset { get; set; }

    [field: SerializeField]
    public float GroundedRadius { get; set; }

    [field: SerializeField]
    public LayerMask GroundLayers { get; set; }

    [field: Header("Timeout")]
    [field: SerializeField]
    public float JumpTimeout { get; set; }

    [field: SerializeField]
    public float FallTimeout { get; set; }

    [field: SerializeField]
    public float LandTimeout { get; set; }

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
        UpdateTimeout(deltaTime);
        ApplyGravity(deltaTime);
        ApplyMove(deltaTime);
        ApplyRotate(deltaTime);
    }

    public void Move(Vector3 direction)
    {
        if (direction != Vector3.zero)
        {
            _movementDirection = direction.normalized;
            _targetSpeed = MoveSpeed;
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

        IsJumping = true;
        IsFalling = false;
        IsLanding = false;
        _verticalVelocity = Mathf.Sqrt(force * -2f * Gravity * GravityMultiplier);
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
            speed = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed, SpeedChangeRate * deltaTime);
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
        var targetRotation = Quaternion.Euler(_rotationDirection);
        var rotation = Quaternion.Slerp(transform.rotation, targetRotation, RotationSpeed * deltaTime);
        transform.rotation = rotation;
    }

    private void ApplyGravity(float deltaTime)
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

    private void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
    }

    private bool IsOnSlope()
    {
        if (IsJumping)
        {
            return false;
        }

        if (Physics.Raycast(transform.position, Vector3.down, out var hit, SlopeRayLength, GroundLayers))
        {
            if (hit.normal != Vector3.up)
            {
                return true;
            }
        }

        return false;
    }

    private void UpdateTimeout(float deltaTime)
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

    private void OnDrawGizmosSelected()
    {
        // CheckGrounded
        var transparentGreen = new Color(0f, 1f, 0f, 0.35f);
        var transparentRed = new Color(1f, 0f, 0f, 0.35f);

        if (IsGrounded)
        {
            Gizmos.color = transparentGreen;
        }
        else
        {
            Gizmos.color = transparentRed;
        }

        var spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, GroundedRadius);

        // Slope Ray
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Vector3.down * SlopeRayLength);
    }
}
