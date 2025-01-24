using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public sealed class UIManager : MonoSingleton<UIManager>
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
    private bool _isInitializedUISettings;

    protected override void Init()
    {
        base.Init();
        CreateCanvases();
    }

    public static void InitUISettings()
    {
        var instance = Instance;
        var settings = UISettings.Instance;

        if (instance._isInitializedUISettings)
        {
            return;
        }

        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            foreach (var prefab in settings[uiType].Prefabs)
            {
                var go = Instantiate(prefab);
                var view = go.GetComponent<UI_View>();
                Add(view);
            }
        }

        Instance._isInitializedUISettings = true;
    }

    public static void Add(UI_View view)
    {
        var instance = Instance;
        var viewType = view.GetType();

        if (!instance._objects.TryGetValue(viewType, out _))
        {
            if (view.UIType == UIType.Popup)
            {
                instance.InitPopup(view as UI_Popup);
            }

            view.transform.SetParent(instance._canvases[view.UIType].transform);
            instance._objects.Add(viewType, view);
        }
        else
        {
            Destroy(view.gameObject);
            Debug.LogWarning($"[UIManager.Add] {viewType} is destroyed because it already exists.");
        }
    }

    public static void Remove<T>() where T : UI_View
    {
        var instance = Instance;

        if (instance._objects.TryGetValue(typeof(T), out var view))
        {
            if (view.UIType == UIType.Popup)
            {
                var popup = view as UI_Popup;

                if (popup.gameObject.activeSelf)
                {
                    if (popup.IsHelper)
                    {
                        instance._helperPopup = null;
                    }
                    else if (popup.IsSelfish)
                    {
                        instance._selfishPopup = null;
                    }
                }

                instance._activePopups.Remove(popup);
            }

            instance._objects.Remove(typeof(T));
            Destroy(view.gameObject);
        }
    }

    public static T Show<T>() where T : UI_View
    {
        var instance = Instance;

        if (instance._objects.TryGetValue(typeof(T), out var view))
        {
            if (view.gameObject.activeSelf)
            {
                return view as T;
            }

            if (view.UIType == UIType.Popup)
            {
                instance.ShowPopup(view as UI_Popup);
                return view as T;
            }
            else
            {
                view.gameObject.SetActive(true);
                return view as T;
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

        if (instance._objects.TryGetValue(typeof(T), out var view))
        {
            if (!view.gameObject.activeSelf)
            {
                return;
            }

            if (view.UIType == UIType.Popup)
            {
                instance.HidePopup(view as UI_Popup);
            }
            else
            {
                view.gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.LogWarning($"[UIManager.Hide] {typeof(T)} does not exist.");
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
        if (Instance._objects.TryGetValue(typeof(T), out var view))
        {
            return view as T;
        }

        return null;
    }

    public static bool Contains<T>() where T : UI_View
    {
        return Instance._objects.ContainsKey(typeof(T));
    }

    public static bool IsActive<T>() where T : UI_View
    {
        return Instance._objects.TryGetValue(typeof(T), out var view) && view.gameObject.activeSelf;
    }

    public static void UpdateCanvases()
    {
        foreach (var canvas in Instance._canvases.Values)
        {
            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(Screen.width, Screen.height);
            canvasScaler.matchWidthOrHeight = 0.5f;
        }
    }

    public static void Clear()
    {
        var instance = Instance;

        foreach (var view in instance._objects.Values)
        {
            Destroy(view.gameObject);
        }

        instance._objects.Clear();
        instance._activePopups.Clear();
        instance._helperPopup = null;
        instance._selfishPopup = null;
        instance._isInitializedUISettings = false;
    }

    private void CreateCanvases()
    {
        var settings = UISettings.Instance;

        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            var go = new GameObject($"{uiType}_Canvas");
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

    private void ShowPopup(UI_Popup popup)
    {
        if (_selfishPopup != null && !popup.IgnoreSelfish)
        {
            return;
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
