using System;
using UnityEngine;

public class UI_ConfirmationPopup : UI_Popup
{
    private DataBinder _binder;

    protected override void Init()
    {
        base.Init();

        _binder = new(gameObject);
        _binder.GetButton("NoButton").onClick.AddListener(UIManager.Close<UI_ConfirmationPopup>);

        UIManager.Register(this);
    }

    public void SetEvent(Action callback, string guideText, string yesText = "예", string noText = "아니오",
        bool activeYes = true, bool activeNo = true)
    {
        var yesButton = _binder.GetButton("YesButton");
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() => callback?.Invoke());
        yesButton.onClick.AddListener(UIManager.Close<UI_ConfirmationPopup>);
        yesButton.gameObject.SetActive(activeYes);
        _binder.GetButton("NoButton").gameObject.SetActive(activeNo);
        _binder.GetText("YesText").text = yesText;
        _binder.GetText("NoText").text = noText;
        _binder.GetText("GuideText").text = guideText;
    }
}
