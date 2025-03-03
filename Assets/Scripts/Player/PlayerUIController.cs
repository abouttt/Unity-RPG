using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void OnCursorToggle(InputValue inputValue)
    {
        var inputManager = Managers.Input;
        inputManager.CursorLocked = !inputManager.CursorLocked;
    }
}
