using System.Text.RegularExpressions;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject go) where T : Component
    {
        go.TryGetComponent(out T component);
        return component;
    }

    public static Transform FindChild(Transform parent, string name, bool recursive = false)
    {
        foreach (Transform child in parent)
        {
            if (child.name.Equals(name))
            {
                return child;
            }
            else if (recursive)
            {
                Transform found = FindChild(child, name, recursive);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    public static Transform FindChildWithTag(Transform parent, string tag, bool recursive = false)
    {
        foreach (Transform child in parent)
        {
            if (child.CompareTag(tag))
            {
                return child;
            }
            else if (recursive)
            {
                Transform found = FindChildWithTag(child, tag, recursive);
                if (found != null)
                {
                    return found;
                }
            }
        }

        return null;
    }

    public static bool IsInLayerMask(GameObject go, LayerMask layerMask)
    {
        return (layerMask.value & (1 << go.layer)) != 0;
    }

    public static void AllIgnoreLayerCollision(GameObject go)
    {
        int layer = go.layer;

        for (int i = 0; i < 32; i++)
        {
            Physics.IgnoreLayerCollision(layer, i, true);
        }
    }

    public static void SetLayerCollision(GameObject go, LayerMask layers, bool ignore)
    {
        int layer = go.layer;

        for (int i = 0; i < 32; i++)
        {
            if ((layers.value & (1 << i)) != 0) // targetLayers에 포함된 레이어인지 확인
            {
                Physics.IgnoreLayerCollision(layer, i, ignore);
            }
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle > 180f)
        {
            return angle - 360f;
        }

        if (angle < -180f)
        {
            return angle + 360f;
        }

        return Mathf.Clamp(angle, min, max);
    }

    public static string ToSnake(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        // 소문자 또는 숫자와 대문자 사이에 언더스코어를 추가
        str = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1_$2");

        // 대문자가 연속된 후, 다음에 나오는 대문자와 소문자 또는 숫자 사이에 언더스코어를 추가
        str = Regex.Replace(str, "([A-Z]+)([A-Z][a-z0-9])", "$1_$2");

        return str;
    }
}
