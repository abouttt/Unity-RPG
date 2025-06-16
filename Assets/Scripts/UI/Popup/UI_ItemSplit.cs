using System;
using UnityEngine;

public class UI_ItemSplit : UI_Popup
{
    public int Quantity { get; private set; }

    private int _price;
    private int _minQuantity;
    private int _maxQuantity;

    protected override void Init()
    {
        base.Init();

        var inputField = GetInputField("InputField");
        inputField.onValueChanged.AddListener(value => OnValueChanged(value));
        inputField.onEndEdit.AddListener(value => OnEndEdit(value));
        inputField.onSubmit.AddListener(value => GetButton("YesButton").onClick?.Invoke());

        GetButton("UpButton").onClick.AddListener(() => OnClickUpOrDownButton(1));
        GetButton("DownButton").onClick.AddListener(() => OnClickUpOrDownButton(-1));
        GetButton("NoButton").onClick.AddListener(Managers.UI.Hide<UI_ItemSplit>);
    }

    public void SetEvent(Action callback, string guideText, int minQuantity, int maxQuantity, int price = -1)
    {
        var yesButton = GetButton("YesButton");
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            Managers.UI.Hide<UI_ItemSplit>();
        });

        Quantity = maxQuantity;
        _maxQuantity = maxQuantity;
        _minQuantity = minQuantity;
        _price = price;

        GetObject("ItemPrice").SetActive(price >= 0);
        GetText("GuideText").text = guideText;

        var inputField = GetInputField("InputField");
        inputField.text = maxQuantity.ToString();
        inputField.ActivateInputField();

        RefreshPriceText();
    }

    public void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        Quantity = Mathf.Clamp(int.Parse(value), _minQuantity, _maxQuantity);
        GetInputField("InputField").text = Quantity.ToString();
        RefreshPriceText();
    }

    public void OnEndEdit(string value)
    {
        Quantity = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxQuantity : int.Parse(value), _minQuantity, _maxQuantity);
        GetInputField("InputField").text = Quantity.ToString();
    }

    public void OnClickUpOrDownButton(int quantity)
    {
        Quantity = Mathf.Clamp(Quantity + quantity, _minQuantity, _maxQuantity);
        GetInputField("InputField").text = Quantity.ToString();
    }

    private void RefreshPriceText()
    {
        if (_price < 0)
        {
            return;
        }

        int totalPrice = _price * Quantity;
        GetText("PriceText").text = totalPrice.ToString();
        // TODO : Player Gold
    }
}
