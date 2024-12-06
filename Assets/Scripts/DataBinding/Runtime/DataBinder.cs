using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public class DataBinder : MonoBehaviour
{
    private readonly Dictionary<Type, Dictionary<string, Object>> _bindings = new();

    private void Awake()
    {
        FindDataBindings();
    }

    public GameObject GetObject(string id) => Get<GameObject>(id);
    public RectTransform GetRectTransform(string id) => Get<RectTransform>(id);
    public Image GetImage(string id) => Get<Image>(id);
    public RawImage GetRawImage(string id) => Get<RawImage>(id);
    public TextMeshProUGUI GetText(string id) => Get<TextMeshProUGUI>(id);
    public Button GetButton(string id) => Get<Button>(id);
    public Toggle GetToggle(string id) => Get<Toggle>(id);
    public Slider GetSlider(string id) => Get<Slider>(id);
    public Scrollbar GetScrollbar(string id) => Get<Scrollbar>(id);
    public ScrollRect GetScrollRect(string id) => Get<ScrollRect>(id);
    public TMP_Dropdown GetDropdown(string id) => Get<TMP_Dropdown>(id);
    public TMP_InputField GetInputField(string id) => Get<TMP_InputField>(id);

    public T Get<T>(string id) where T : Object
    {
        if (_bindings.TryGetValue(typeof(T), out var type))
        {
            if (type.TryGetValue(id, out var component))
            {
                return component as T;
            }
        }

        return null;
    }

    private void FindDataBindings()
    {
        foreach (var binding in gameObject.GetComponentsInChildren<DataBinding>(true))
        {
            if (IsNullBinding(binding))
            {
                LogWarning($"Binding failed : ID or Target is null", binding);
                continue;
            }

            AddBinding(binding);
        }
    }

    private void AddBinding(DataBinding binding)
    {
        if (!_bindings.TryGetValue(binding.BindingType, out var typeBindings))
        {
            typeBindings = new Dictionary<string, Object>();
            _bindings[binding.BindingType] = typeBindings;
        }

        if (typeBindings.ContainsKey(binding.DataID))
        {
            LogWarning($"Binding failed : Duplicate ID", binding);
            return;
        }

        typeBindings.Add(binding.DataID, binding.Target);
    }

    private bool IsNullBinding(DataBinding binding)
    {
        return string.IsNullOrEmpty(binding.DataID) || binding.Target == null;
    }

    private void LogWarning(string message, DataBinding binding)
    {
        Debug.LogWarning($"[DataBinder] {gameObject.name} - {message} (ID : {binding.DataID}, Target : {binding.Target})");
    }
}
