using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputValue : MonoBehaviour
{
    public Vector2 Move;
    public Vector2 Look;
    public bool Sprint;
    public bool Jump;

    public void OnMove(InputValue inputValue)
    {
        Move = inputValue.Get<Vector2>();
    }

    public void OnLook(InputValue inputValue)
    {
        Look = InputManager.CursorLocked ? inputValue.Get<Vector2>() : Vector2.zero;
    }

    public void OnSprint(InputValue inputValue)
    {
        Sprint = inputValue.isPressed;
    }

    public void OnJump(InputValue inputValue)
    {
        Jump = inputValue.isPressed;
    }
}
