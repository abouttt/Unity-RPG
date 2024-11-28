using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Object = UnityEngine.Object;

public class UIBinder
{
    private readonly Dictionary<Type, List<Object>> _objects = new();

    private GameObject _gameObject;

    public UIBinder(GameObject gameObject)
    {
        _gameObject = gameObject;
    }

    // 본인을 포함한 자식 오브젝트에서 type을 찾아 바인딩하고 이미 존재하는 type이면 뒤이어 추가한다.
    public void Bind<T>(Type type) where T : Object
    {
        var names = Enum.GetNames(type);
        var newObjects = new List<Object>(names.Length);

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                newObjects.Add(_gameObject.FindChild(names[i], true));
            }
            else
            {
                newObjects.Add(_gameObject.FindChild<T>(names[i], true));
            }

            if (newObjects[i] == null)
            {
                Debug.LogWarning($"{_gameObject.name} failed to bind({names[i]})");
            }
        }

        if (_objects.TryGetValue(typeof(T), out var objects))
        {
            objects.AddRange(newObjects);
        }
        else
        {
            _objects.Add(typeof(T), newObjects);
        }
    }

    public void BindObject(Type type) => Bind<GameObject>(type);
    public void BindRT(Type type) => Bind<RectTransform>(type);
    public void BindImage(Type type) => Bind<Image>(type);
    public void BindText(Type type) => Bind<TextMeshProUGUI>(type);
    public void BindButton(Type type) => Bind<Button>(type);
    public void BindToggle(Type type) => Bind<Toggle>(type);
    public void BindSlider(Type type) => Bind<Slider>(type);
    public void BindScrollbar(Type type) => Bind<Scrollbar>(type);
    public void BindScrollRect(Type type) => Bind<ScrollRect>(type);
    public void BindDropdown(Type type) => Bind<TMP_Dropdown>(type);
    public void BindInputField(Type type) => Bind<TMP_InputField>(type);

    public T Get<T>(int index) where T : Object
    {
        if (_objects.TryGetValue(typeof(T), out var objects))
        {
            return objects[index] as T;
        }

        return null;
    }

    public GameObject GetObject(int index) => Get<GameObject>(index);
    public RectTransform GetRT(int index) => Get<RectTransform>(index);
    public Image GetImage(int index) => Get<Image>(index);
    public TextMeshProUGUI GetText(int index) => Get<TextMeshProUGUI>(index);
    public Button GetButton(int index) => Get<Button>(index);
    public Toggle GetToggle(int index) => Get<Toggle>(index);
    public Slider GetSlider(int index) => Get<Slider>(index);
    public Scrollbar GetScrollbar(int index) => Get<Scrollbar>(index);
    public ScrollRect GetScrollRect(int index) => Get<ScrollRect>(index);
    public TMP_Dropdown GetDropdown(int index) => Get<TMP_Dropdown>(index);
    public TMP_InputField GetInputField(int index) => Get<TMP_InputField>(index);
}
