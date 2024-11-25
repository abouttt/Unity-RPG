using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ConfirmationPopup : UI_Popup
{
    [SerializeField]
    private TextMeshProUGUI _guideText;

    [SerializeField]
    private TextMeshProUGUI _yesText;

    [SerializeField]
    private TextMeshProUGUI _noText;

    [SerializeField]
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    protected override void Init()
    {
        base.Init();
        _noButton.onClick.AddListener(UIManager.Close<UI_ConfirmationPopup>);
        UIManager.Register(this);
    }

    public void SetEvent(Action callback, string guideText, string yesText = "예", string noText = "아니오",
        bool activeYes = true, bool activeNo = true)
    {
        _yesButton.onClick.RemoveAllListeners();
        _yesButton.onClick.AddListener(() => callback?.Invoke());
        _yesButton.onClick.AddListener(UIManager.Close<UI_ConfirmationPopup>);
        _yesButton.gameObject.SetActive(activeYes);
        _noButton.gameObject.SetActive(activeNo);
        _yesText.text = yesText;
        _noText.text = noText;
        _guideText.text = guideText;
    }
}
