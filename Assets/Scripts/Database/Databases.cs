using UnityEngine;

public static class Databases
{
    public static ItemDatabase Item { get; private set; }

    public static void Initialize()
    {
        Item = Load<ItemDatabase>();
    }

    private static T Load<T>() where T : ScriptableObject
    {
        var asset = Resources.Load<T>($"Databases/{typeof(T).Name}");
        if (asset == null)
        {
            Debug.LogWarning($"[Databases] Failed load {typeof(T).Name} asset.");
        }

        return asset;
    }
}
