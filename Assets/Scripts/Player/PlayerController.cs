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

    private bool _isJumped;
    private bool _isJumpedWithInput;

    private GameObject _mainCamera;
    private PlayerInputValue _input;
    private GroundedCharacterController _movement;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _input = GetComponent<PlayerInputValue>();
        _movement = GetComponent<GroundedCharacterController>();
    }

    private void Update()
    {
        UpdateMoveSpeed();
        CheckJump();
        Move();
        Rotate();
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
            float cameraYaw = _mainCamera.transform.eulerAngles.y;
            float yaw = GetYaw(inputDirection) + cameraYaw;
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

    private float GetYaw(Vector3 direction)
    {
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
}
