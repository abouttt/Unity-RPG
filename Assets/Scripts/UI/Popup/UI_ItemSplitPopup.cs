using System;
using UnityEngine;

public class UI_ItemSplitPopup : UI_Popup
{
    public int Quantity { get; private set; }

    private DataBinder _binder;

    private int _price;
    private int _minQuantity;
    private int _maxQuantity;

    protected override void Init()
    {
        base.Init();

        _binder = new(gameObject);

        var inputField = _binder.GetInputField("InputField");
        inputField.onValueChanged.AddListener(value => OnValueChanged(value));
        inputField.onEndEdit.AddListener(value => OnEndEdit(value));
        inputField.onSubmit.AddListener(value => _binder.GetButton("YesButton").onClick?.Invoke());

        _binder.GetButton("UpButton").onClick.AddListener(() => OnClickUpOrDownButton(1));
        _binder.GetButton("DownButton").onClick.AddListener(() => OnClickUpOrDownButton(-1));
        _binder.GetButton("NoButton").onClick.AddListener(UIManager.Close<UI_ItemSplitPopup>);

        UIManager.Register(this);
    }

    public void SetEvent(Action callback, string guideText, int minQuantity, int maxQuantity, int price = -1)
    {
        var yesButton = _binder.GetButton("YesButton");
        yesButton.onClick.RemoveAllListeners();
        yesButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            UIManager.Close<UI_ItemSplitPopup>();
        });

        Quantity = maxQuantity;
        _maxQuantity = maxQuantity;
        _minQuantity = minQuantity;
        _price = price;

        _binder.GetObject("ItemPrice").SetActive(price >= 0);
        _binder.GetText("GuideText").text = guideText;

        var inputField = _binder.GetInputField("InputField");
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
        _binder.GetInputField("InputField").text = Quantity.ToString();
        RefreshPriceText();
    }

    public void OnEndEdit(string value)
    {
        Quantity = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxQuantity : int.Parse(value), _minQuantity, _maxQuantity);
        _binder.GetInputField("InputField").text = Quantity.ToString();
    }

    public void OnClickUpOrDownButton(int quantity)
    {
        Quantity = Mathf.Clamp(Quantity + quantity, _minQuantity, _maxQuantity);
        _binder.GetInputField("InputField").text = Quantity.ToString();
    }

    private void RefreshPriceText()
    {
        if (_price < 0)
        {
            return;
        }

        int totalPrice = _price * Quantity;
        _binder.GetText("PriceText").text = totalPrice.ToString();
        // TODO : Player Gold
    }
}
