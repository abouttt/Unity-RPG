using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    private void OnCursorToggle(InputValue inputValue)
    {
        InputManager.CursorLocked = !InputManager.CursorLocked;
    }

    private void OnItemInventory(InputValue inputValue)
    {
        ShowOrClosePopup<UI_ItemInventoryPopup>();
    }

    private void OnCancel(InputValue inputValue)
    {
        if (UIManager.ActivePopupCount > 0)
        {
            UIManager.CloseTopPopup();
        }
    }

    private void ShowOrClosePopup<T>() where T : UI_Popup
    {
        if (UIManager.IsActiveHelperPopup)
        {
            return;
        }

        UIManager.ShowOrClose<T>();
    }
}
