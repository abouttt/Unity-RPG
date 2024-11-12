using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        if (go.TryGetComponent<T>(out var component))
        {
            return component;
        }

        return go.AddComponent<T>();
    }

    public static Transform FindChildWithTag(Transform parent, string tag)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
        }

        return null;
    }

    public static bool IsInLayerMask(GameObject go, LayerMask layerMask)
    {
        return (layerMask.value & (1 << go.layer)) != 0;
    }
}
