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

    private GameObject _mainCamera;
    private Animator _animator;
    private CharacterMovement _movement;

    // Input Value
    private Vector2 _move;
    private bool _isPressedSprint;

    private void Awake()
    {
        _mainCamera = Camera.main.gameObject;
        _animator = GetComponent<Animator>();
        _movement = GetComponent<CharacterMovement>();
    }

    private void Update()
    {
        MoveAndRotate();
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
        _movement.Rotate(inputDirection, cameraYaw);
    }

    // Input

    private void OnMove(InputValue inputValue)
    {
        _move = inputValue.Get<Vector2>();
    }

    private void OnSprint(InputValue inputValue)
    {
        _isPressedSprint = inputValue.isPressed;
    }

    private void OnJump(InputValue inputValue)
    {
        _movement.Jump();
    }
}
