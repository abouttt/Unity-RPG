using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static Transform FindChildWithTag(this Transform parent, string tag)
    {
        return Util.FindChildWithTag(parent, tag);
    }

    public static bool IsInLayerMask(this GameObject go, LayerMask layerMask)
    {
        return Util.IsInLayerMask(go, layerMask);
    }
}
