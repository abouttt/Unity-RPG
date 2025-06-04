using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public void OnCursorToggle()
    {
        var input = Managers.Input;
        input.CursorLocked = !input.CursorLocked;
    }
}
