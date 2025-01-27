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
    private float _jumpForce;

    // Input Value
    private Vector2 _move;
    private Vector2 _look;
    private bool _isPressedSprint;

    private GameObject _mainCamera;
    private GroundedCharacterController _controller;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _controller = GetComponent<GroundedCharacterController>();
    }

    private void Update()
    {
        UpdateMoveSpeed();
        MoveAndRotate();
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

            _controller.Move(movementDirection);
            _controller.Rotate(rotationDirection);
        }
        else
        {
            _controller.Move(Vector3.zero);
        }
    }

    private void UpdateMoveSpeed()
    {
        if (_controller.IsGrounded)
        {
            _controller.MoveSpeed = _controller.IsLanding ? _landingSpeed
                                  : _isPressedSprint ? _sprintSpeed
                                  : _runSpeed;
        }
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
        _controller.Jump(_jumpForce);
    }
}
