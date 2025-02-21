using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void OnCursorToggle(InputValue inputValue)
    {
        InputManager.CursorLocked = !InputManager.CursorLocked;
    }
}
