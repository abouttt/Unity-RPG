using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    private void OnCursorToggle(InputValue inputValue)
    {
        InputManager.CursorLocked = !InputManager.CursorLocked;
    }
}
