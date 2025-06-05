using UnityEngine;

public static class VectorUtility
{
    public const float Epsilon = 1e-5f;

    public static bool IsNearlyEqual(this Vector2 a, Vector2 b, float tolerance = Epsilon)
    {
        return (a - b).sqrMagnitude < tolerance;
    }

    public static bool IsNearlyEqual(this Vector3 a, Vector3 b, float tolerance = Epsilon)
    {
        return (a - b).sqrMagnitude < tolerance;
    }

    public static bool IsNearlyZero(this Vector2 v, float tolerance = Epsilon)
    {
        return v.sqrMagnitude < tolerance;
    }

    public static bool IsNearlyZero(this Vector3 v, float tolerance = Epsilon)
    {
        return v.sqrMagnitude < tolerance;
    }
}
