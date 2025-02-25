using UnityEngine;

public static class Settings
{
    public static SceneSettings Scene
    {
        get
        {
            if (_scene == null)
            {
                _scene = Load<SceneSettings>();
            }

            return _scene;
        }
    }

    public static UISettings UI
    {
        get
        {
            if (_ui == null)
            {
                _ui = Load<UISettings>();
            }

            return _ui;
        }
    }

    private static SceneSettings _scene;
    private static UISettings _ui;

    private static T Load<T>() where T : ScriptableObject
    {
        var asset = Resources.Load<T>($"Settings/{typeof(T).Name}");
        if (asset == null)
        {
            Debug.LogWarning($"[Settings] Can't find {typeof(T).Name} asset.");
        }

        return asset;
    }
}
