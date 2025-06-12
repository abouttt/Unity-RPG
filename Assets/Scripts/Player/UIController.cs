using UnityEngine;
using UnityEngine.InputSystem;

public class UIController : MonoBehaviour
{
    public void OnCursorToggle()
    {
        var input = Managers.Input;
        input.CursorLocked = !input.CursorLocked;
    }

    public void OnCancel()
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
