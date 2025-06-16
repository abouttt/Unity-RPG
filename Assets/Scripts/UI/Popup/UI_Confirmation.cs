using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Confirmation : UI_Popup
{
    protected override void Init()
    {
        base.Init();
        GetButton("NoButton").onClick.AddListener(Managers.UI.Hide<UI_Confirmation>);
        Showed += () => EventSystem.current.SetSelectedGameObject(GetButton("YesButton").gameObject);
    }

    public void SetEvent(Action callback, string guideText, string yesText = "��", string noText = "�ƴϿ�",
        bool activeYes = true, bool activeNo = true)
    {
        var yesButton = GetButton("YesButton");
        yesButton.gameObject.SetActive(activeYes);
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            Managers.UI.Hide<UI_Confirmation>();
        });

        GetButton("NoButton").gameObject.SetActive(activeNo);
        GetText("YesText").text = yesText;
        GetText("NoText").text = noText;
        GetText("GuideText").text = guideText;
    }
}
