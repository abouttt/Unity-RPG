using UnityEngine;
using UnityEngine.InputSystem;

public sealed class InputManager : MonoSingleton<InputManager>
{
    public static bool Enabled
    {
        get => Instance._inputActions.asset.enabled;
        set
        {
            if (value)
            {
                Instance._inputActions.Enable();
            }
            else
            {
                Instance._inputActions.Disable();
            }
        }
    }

    public static bool CursorLocked
    {
        get => Cursor.lockState == CursorLockMode.Locked;
        set
        {
            Cursor.visible = !value;
            Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private InputActions _inputActions;

    protected override void Init()
    {
        base.Init();
        _inputActions = new();
        Enabled = false;
        CursorLocked = false;
    }

    public static InputActionMap GetActionMap(string nameOrId, bool throwIfNotFound = false)
    {
        return Instance._inputActions.asset.FindActionMap(nameOrId, throwIfNotFound);
    }

    public static InputAction GetAction(string actionNameOrId, bool throwIfNotFound = false)
    {
        return Instance._inputActions.FindAction(actionNameOrId, throwIfNotFound);
    }

    public static string GetBindingPath(string actionNameOrId, int bindingIndex = 0)
    {
        var bindingPath = GetAction(actionNameOrId).bindings[bindingIndex].path;
        int lastSlashIndex = bindingPath.LastIndexOf('/');
        var path = lastSlashIndex >= 0 ? bindingPath[(lastSlashIndex + 1)..] : bindingPath;
        return path.ToUpper();
    }

    public static void Clear()
    {
        Enabled = false;
    }
}
