using UnityEngine;

public static class Managers
{
    public static InputManager Input => InputManager.Instance;
    public static PoolManager Pool => PoolManager.Instance;
    public static ResourceManager Resource => ResourceManager.Instance;
    public static SceneLoadManager Scene => SceneLoadManager.Instance;
    public static SoundManager Sound => SoundManager.Instance;
    public static UIManager UI => UIManager.Instance;

    public static CooldownManager Cooldown => CooldownManager.Instance;

    public static void Initialize()
    {
        Input.Initialize();
        Pool.Initialize();
        Resource.Initialize();
        Scene.Initialize();
        Sound.Initialize();
        UI.Initialize();

        Cooldown.Initialize();
    }

    public static void Clear()
    {
        Input.Clear();
        Pool.Clear();
        Resource.Clear();
        Scene.Clear();
        Sound.Clear();
        UI.Clear();

        Cooldown.Clear();
    }
}
