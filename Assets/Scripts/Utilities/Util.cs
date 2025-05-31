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

    public static float WrapAngle(float angle)
    {
        angle %= 360f;
        return angle > 180f ? angle - 360f : angle;
    }

    public static float UnwrapAngle(float angle)
    {
        angle %= 360f;
        return angle < 0f ? angle + 360f : angle;
    }

    public static string ToSnake(string str)
    {
        if (string.IsNullOrEmpty(str))
        {
            return str;
        }

        // �ҹ��� �Ǵ� ���ڿ� �빮�� ���̿� ������ھ �߰�
        str = Regex.Replace(str, "([a-z0-9])([A-Z])", "$1_$2");

        // �빮�ڰ� ���ӵ� ��, ������ ������ �빮�ڿ� �ҹ��� �Ǵ� ���� ���̿� ������ھ �߰�
        str = Regex.Replace(str, "([A-Z]+)([A-Z][a-z0-9])", "$1_$2");

        return str;
    }
}
