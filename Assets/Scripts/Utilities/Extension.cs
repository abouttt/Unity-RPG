using UnityEngine;

public static class Extension
{
    public static T GetOrAddComponent<T>(this GameObject go) where T : Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    public static Transform FindChild(this Transform parent, string name, bool recursive = false)
    {
        return Util.FindChild(parent, name, recursive);
    }

    public static Transform FindChildWithTag(this Transform parent, string tag, bool recursive = false)
    {
        return Util.FindChildWithTag(parent, tag, recursive);
    }

    public static bool IsInLayerMask(this GameObject go, LayerMask layerMask)
    {
        return Util.IsInLayerMask(go, layerMask);
    }

    public static void AllIgnoreLayerCollision(this GameObject go)
    {
        Util.AllIgnoreLayerCollision(go);
    }

    public static void SetLayerCollision(this GameObject go, LayerMask layerMask, bool ignore)
    {
        Util.SetLayerCollision(go, layerMask, ignore);
    }

    public static string ToSnake(this string str)
    {
        return Util.ToSnake(str);
    }
}
