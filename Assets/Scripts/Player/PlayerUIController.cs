using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void OnCursorToggle(InputValue inputValue)
    {
        InputManager.CursorLocked = !InputManager.CursorLocked;
    }

    public void OnItemInventory(InputValue inputValue)
    {
        ShowOrHidePopup<UI_ItemInventory>();
    }

    public void OnCancel(InputValue inputValue)
    {
        UIManager.HideTopPopup();
    }

    private void ShowOrHidePopup<T>() where T : UI_Popup
    {
        if (UIManager.IsActiveHelperPopup)
        {
            return;
        }

        UIManager.Toggle<T>();
    }
}
