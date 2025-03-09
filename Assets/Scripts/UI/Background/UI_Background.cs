using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Background : UI_View, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        Managers.Input.CursorLocked = true;
    }
}
