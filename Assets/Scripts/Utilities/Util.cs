using System.Text.RegularExpressions;
using UnityEngine;

public static class Util
{
    public static T GetOrAddComponent<T>(GameObject gameObject) where T : Component
    {
        if (gameObject.TryGetComponent<T>(out var component))
        {
            return component;
        }

        return gameObject.AddComponent<T>();
    }

    public static Transform FindChild(Transform parent, string name, bool recursive = false)
    {
        foreach (Transform child in parent)
        {
            if (child.name == name)
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

    public static bool IsInLayerMask(GameObject gameObject, LayerMask layerMask)
    {
        return (layerMask.value & (1 << gameObject.layer)) != 0;
    }

    public static void SetIgnoreCollision(LayerMask layerMask, bool ignore)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) == 0)
            {
                continue;
            }

            for (int j = 0; j < 32; j++)
            {
                Physics.IgnoreLayerCollision(i, j, ignore);
            }
        }
    }

    public static void SetIgnoreCollision(LayerMask layerMask1, LayerMask layerMask2, bool ignore)
    {
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask1.value & (1 << i)) == 0)
            {
                continue;
            }

            for (int j = 0; j < 32; j++)
            {
                if ((layerMask2.value & (1 << j)) == 0)
                {
                    continue;
                }

                Physics.IgnoreLayerCollision(i, j, ignore);
            }
        }
    }

    public static float WrapAngle(float angle)
    {
        angle %= 360f;

        if (angle > 180f)
        {
            angle -= 360f;
        }
        else if (angle < -180f)
        {
            angle += 360f;
        }

        return angle;
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        angle = WrapAngle(angle);
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
