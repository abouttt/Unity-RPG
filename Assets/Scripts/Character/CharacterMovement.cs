using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class CharacterMovement : MonoBehaviour
{
    public float SpeedBlend {  get; private set; }
    public float PosXBlend { get; private set; }
    public float PosYBlend { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsGrounded { get; private set; } = true;

    [field: SerializeField, ReadOnly]
    public bool IsJumping { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsFalling { get; private set; }

    [field: Header("[Move]")]
    [field: SerializeField]
    public float MoveSpeed { get; set; }

    [field: SerializeField]
    public float SpeedChangeRate { get; set; }

    [field: Header("[Rotation]")]
    [field: SerializeField, Range(0f, 0.3f)]
    public float RotationSpeed { get; set; }

    [Header("[Jump]")]
    [SerializeField]
    private float _jumpHeight;

    [SerializeField]
    private float _gravity;

    [SerializeField]
    private float _jumpTimeout;

    [SerializeField]
    private float _fallTimeout;

    [Header("[Grounded]")]
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
    private readonly float _terminalVelocity = 53f;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    private CharacterController _controller;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public void Gravity()
    {
        if (IsGrounded)
        {
            // 추락 제한시간 리셋
            _fallTimeoutDelta = _fallTimeout;

            IsFalling = false;

            // 착지했을 때 속도가 무한히 떨어지는 것을 방지
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            // 점프 제한시간
            if (_jumpTimeoutDelta >= 0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // 점프 제한시간 리셋
            _jumpTimeoutDelta = _jumpTimeout;

            // 추락 제한시간
            if (_fallTimeoutDelta >= 0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                IsJumping = false;
                IsFalling = true;
            }
        }

        // 터미널 아래에 있는 경우 시간에 따라 중력을 적용
        if (_verticalVelocity < _terminalVelocity)
        {
            _verticalVelocity += _gravity * Time.deltaTime;
        }
    }

    public void MoveAndRotate(Vector3 direction, float overrideYaw = 0f)
    {
        Move(direction, overrideYaw);
        Rotate(direction, overrideYaw);
    }

    public void Move(Vector3 direction, float overrideYaw = 0f)
    {
        float targetSpeed = MoveSpeed;

        if (direction == Vector3.zero)
        {
            targetSpeed = 0f;
        }

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0f, _controller.velocity.z).magnitude;
        float currentSpeedChangeRate = SpeedChangeRate * Time.deltaTime;
        float speedOffset = 0.1f;

        // 목표 속도까지 가감속
        if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            currentHorizontalSpeed > targetSpeed + speedOffset)
        {
            _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed, currentSpeedChangeRate);
            _speed = Mathf.Round(_speed * 1000f) / 1000f;
        }
        else
        {
            _speed = targetSpeed;
        }

        SpeedBlend = Mathf.Lerp(SpeedBlend, targetSpeed, currentSpeedChangeRate);
        PosXBlend = Mathf.Lerp(PosXBlend, direction.x, currentSpeedChangeRate);
        PosYBlend = Mathf.Lerp(PosYBlend, direction.z, currentSpeedChangeRate);
        if (SpeedBlend < 0.01f)
        {
            SpeedBlend = 0f;
            PosXBlend = 0f;
            PosYBlend = 0f;
        }

        if (direction != Vector3.zero)
        {
            _targetMove = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + overrideYaw;
        }

        var targetDirection = Quaternion.Euler(0f, _targetMove, 0f) * Vector3.forward;
        _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0f, _verticalVelocity, 0f) * Time.deltaTime);
    }

    public void Rotate(Vector3 direction, float overrideYaw = 0f)
    {
        if (direction != Vector3.zero)
        {
            _targetRotation = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + overrideYaw;
        }

        float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, RotationSpeed);
        transform.rotation = Quaternion.Euler(0f, rotation, 0f);
    }

    public void Jump()
    {
        if (_jumpTimeoutDelta <= 0f)
        {
            IsJumping = true;
            _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        }
    }

    public void CheckGrounded()
    {
        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        IsGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    // IsGrounded 시각화
    private void OnDrawGizmosSelected()
    {
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

        var spherePosition = new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z);
        Gizmos.DrawSphere(spherePosition, _groundedRadius);
    }
}
