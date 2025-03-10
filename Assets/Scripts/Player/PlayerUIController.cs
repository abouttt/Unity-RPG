using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerUIController : MonoBehaviour
{
    public void OnCursorToggle(InputValue inputValue)
    {
        var inputManager = Managers.Input;
        inputManager.CursorLocked = !inputManager.CursorLocked;
    }

    public void OnItemInventory(InputValue inputValue)
    {
        ShowOrHidePopup<UI_ItemInventory>();
    }

    public void OnCancel(InputValue inputValue)
    {
        Managers.UI.HideTopPopup();
    }

    private void ShowOrHidePopup<T>() where T : UI_Popup
    {
        if (Managers.UI.IsActiveHelperPopup)
        {
            return;
        }

        Managers.UI.Toggle<T>();
    }
}
