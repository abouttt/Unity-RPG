using System;
using UnityEngine;

public class UI_Confirmation : UI_Popup
{
    protected override void Init()
    {
        base.Init();
        GetButton("NoButton").onClick.AddListener(UIManager.Hide<UI_Confirmation>);
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
            UIManager.Hide<UI_Confirmation>();
        });

        GetButton("NoButton").gameObject.SetActive(activeNo);
        GetText("YesText").text = yesText;
        GetText("NoText").text = noText;
        GetText("GuideText").text = guideText;
    }
}
