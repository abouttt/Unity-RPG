using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_ItemSplitPopup : UI_Popup
{
    public int Quantity { get; private set; }

    [SerializeField]
    private TMP_InputField _inputField;

    [SerializeField]
    private TextMeshProUGUI _guideText;

    [SerializeField]
    private Button _upButton;

    [SerializeField]
    private Button _downButton;

    [SerializeField]
    private Button _yesButton;

    [SerializeField]
    private Button _noButton;

    [SerializeField]
    private GameObject _itemPrice;

    [SerializeField]
    private TextMeshProUGUI _priceText;

    private int _price;
    private int _minQuantity;
    private int _maxQuantity;

    protected override void Init()
    {
        base.Init();

        _inputField.onValueChanged.AddListener(value => OnValueChanged(value));
        _inputField.onEndEdit.AddListener(value => OnEndEdit(value));
        _inputField.onSubmit.AddListener(value => _yesButton.onClick?.Invoke());

        _upButton.onClick.AddListener(() => OnClickUpOrDownButton(1));
        _downButton.onClick.AddListener(() => OnClickUpOrDownButton(-1));
        _noButton.onClick.AddListener(UIManager.Close<UI_ItemSplitPopup>);

        UIManager.Register(this);
    }

    public void SetEvent(Action callback, string guideText, int minQuantity, int maxQuantity, int price = -1)
    {
        _yesButton.onClick.RemoveAllListeners();
        _yesButton.onClick.AddListener(() =>
        {
            callback?.Invoke();
            UIManager.Close<UI_ItemSplitPopup>();
        });

        Quantity = maxQuantity;
        _maxQuantity = maxQuantity;
        _minQuantity = minQuantity;
        _price = price;

        _itemPrice.SetActive(price >= 0);
        _guideText.text = guideText;
        _inputField.text = maxQuantity.ToString();
        _inputField.ActivateInputField();

        RefreshPriceText();
    }

    public void OnValueChanged(string value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        Quantity = Mathf.Clamp(int.Parse(value), _minQuantity, _maxQuantity);
        _inputField.text = Quantity.ToString();
        RefreshPriceText();
    }

    public void OnEndEdit(string value)
    {
        Quantity = Mathf.Clamp(string.IsNullOrEmpty(value) ? _maxQuantity : int.Parse(value), _minQuantity, _maxQuantity);
        _inputField.text = Quantity.ToString();
    }

    public void OnClickUpOrDownButton(int quantity)
    {
        Quantity = Mathf.Clamp(Quantity + quantity, _minQuantity, _maxQuantity);
        _inputField.text = Quantity.ToString();
    }

    private void RefreshPriceText()
    {
        if (_price < 0)
        {
            return;
        }

        int totalPrice = _price * Quantity;
        _priceText.text = totalPrice.ToString();
        // TODO : Player Gold
    }
}
