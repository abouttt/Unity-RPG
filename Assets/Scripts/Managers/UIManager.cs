using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoBehaviourSingleton<UIManager>
{
    public static int Count => Instance._objects.Count;
    public static int ActivePopupCount => Instance._activePopups.Count;
    public static bool IsActiveHelperPopup => Instance._helperPopup != null;
    public static bool IsActiveSelfishPopup => Instance._selfishPopup != null;

    private readonly Dictionary<UIType, Canvas> _canvases = new();
    private readonly Dictionary<Type, UI_View> _objects = new();
    private readonly LinkedList<UI_Popup> _activePopups = new();
    private UI_Popup _helperPopup;
    private UI_Popup _selfishPopup;

    protected override void Init()
    {
        base.Init();
        CreateCanvases();
    }

    protected override void Dispose()
    {
        base.Dispose();

        foreach (var canvas in _canvases.Values)
        {
            Destroy(canvas.gameObject);
        }

        _canvases.Clear();
        _objects.Clear();
        _activePopups.Clear();
    }

    public static void CreateUI()
    {
        var instance = Instance;
        var settings = UISettings.Instance;

        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            foreach (var prefab in settings[uiType].Prefabs)
            {
                var ui = Instantiate(prefab);
                instance.Add(ui);
            }
        }
    }

    public static T Show<T>() where T : UI_View
    {
        var instance = Instance;

        if (instance._objects.TryGetValue(typeof(T), out var ui))
        {
            if (ui.gameObject.activeSelf)
            {
                return ui as T;
            }

            if (ui.UIType == UIType.Popup)
            {
                return instance.ShowPopup<T>(ui as UI_Popup);
            }
            else
            {
                ui.gameObject.SetActive(true);
                return ui as T;
            }
        }
        else
        {
            Debug.LogWarning($"[UIManager.Show] {typeof(T)} does not exist.");
            return null;
        }
    }

    public static void Hide<T>() where T : UI_View
    {
        var instance = Instance;

        if (instance._objects.TryGetValue(typeof(T), out var ui))
        {
            if (!ui.gameObject.activeSelf)
            {
                return;
            }

            if (ui.UIType == UIType.Popup)
            {
                instance.HidePopup(ui as UI_Popup);
            }
            else
            {
                ui.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"[UIManager.Hide] {typeof(T)} does not exist.");
        }
    }

    public static void HideTopPopup()
    {
        var instance = Instance;

        if (instance._activePopups.Count > 0)
        {
            instance.HidePopup(instance._activePopups.First.Value);
        }
    }

    public static void HideAll(UIType uiType)
    {
        var instance = Instance;

        if (uiType == UIType.Popup)
        {
            foreach (var popup in instance._activePopups)
            {
                popup.gameObject.SetActive(false);
            }

            instance._activePopups.Clear();
            instance._helperPopup = null;
            instance._selfishPopup = null;
        }
        else
        {
            foreach (var canvas in instance._canvases.Values)
            {
                canvas.gameObject.SetActive(false);
            }
        }
    }

    public static void Toggle<T>() where T : UI_View
    {
        if (IsActive<T>())
        {
            Hide<T>();
        }
        else
        {
            Show<T>();
        }
    }

    public static T Get<T>() where T : UI_View
    {
        if (Instance._objects.TryGetValue(typeof(T), out var ui))
        {
            return ui as T;
        }

        return null;
    }

    public static bool Contains<T>() where T : UI_View
    {
        return Instance._objects.ContainsKey(typeof(T));
    }

    public static bool IsActive<T>() where T : UI_View
    {
        return Instance._objects.TryGetValue(typeof(T), out var ui) && ui.gameObject.activeSelf;
    }

    public static void Clear()
    {
        var instance = Instance;

        foreach (var ui in instance._objects.Values)
        {
            Destroy(ui.gameObject);
        }

        instance._objects.Clear();
        instance._activePopups.Clear();
        instance._helperPopup = null;
        instance._selfishPopup = null;
    }

    private void CreateCanvases()
    {
        var settings = UISettings.Instance;

        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            var go = new GameObject($"{uiType}Canvas");
            var canvas = go.AddComponent<Canvas>();
            var canvasScaler = go.AddComponent<CanvasScaler>();

            if (settings[uiType].GraphicRaycaster)
            {
                go.AddComponent<GraphicRaycaster>();
            }

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = (int)uiType;

            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.matchWidthOrHeight = 0.5f;

            go.transform.SetParent(transform);

            _canvases.Add(uiType, canvas);
        }
    }

    private void Add<T>(T ui) where T : UI_View
    {
        var instance = Instance;

        if (!instance._objects.TryGetValue(typeof(T), out _))
        {
            if (ui.UIType == UIType.Popup)
            {
                instance.InitPopup(ui as UI_Popup);
            }

            ui.transform.SetParent(instance._canvases[ui.UIType].transform);
            instance._objects.Add(typeof(T), ui);
        }
        else
        {
            Destroy(ui.gameObject);
            Debug.LogWarning($"[UIManager.Add] {typeof(T)} is destroyed because it already exists.");
        }
    }

    private void InitPopup(UI_Popup popup)
    {
        popup.Focused += () =>
        {
            if (_activePopups.First.Value == popup)
            {
                return;
            }

            _activePopups.Remove(popup);
            _activePopups.AddFirst(popup);
            RefreshAllPopupDepth();
        };

        popup.Showed += () =>
        {
            InputManager.CursorLocked = false;
        };

        popup.Hided += () =>
        {
            if (_activePopups.Count == 0)
            {
                InputManager.CursorLocked = true;
            }
        };
    }

    private T ShowPopup<T>(UI_Popup popup) where T : UI_View
    {
        if (_selfishPopup != null && !popup.IgnoreSelfish)
        {
            return null;
        }

        if (popup.IsHelper)
        {
            if (_helperPopup != null)
            {
                _activePopups.Remove(_helperPopup);
                _helperPopup.gameObject.SetActive(false);
            }

            _helperPopup = popup;
        }
        else if (popup.IsSelfish)
        {
            HideAll(UIType.Popup);
            _selfishPopup = popup;
        }

        _activePopups.AddFirst(popup);
        RefreshAllPopupDepth();
        popup.gameObject.SetActive(true);

        return popup as T;
    }

    private void HidePopup(UI_Popup popup)
    {
        if (popup.IsHelper)
        {
            _helperPopup = null;
        }
        else if (popup.IsSelfish)
        {
            _selfishPopup = null;
        }

        _activePopups.Remove(popup);
        popup.gameObject.SetActive(false);
    }

    private void RefreshAllPopupDepth()
    {
        foreach (var popup in _activePopups)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
