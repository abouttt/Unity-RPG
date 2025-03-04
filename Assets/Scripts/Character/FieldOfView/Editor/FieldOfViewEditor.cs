using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        var fov = (FieldOfView)target;
        var transform = fov.transform;

        Handles.color = Color.white;
        Handles.DrawWireArc(transform.position, Vector3.up, Vector3.forward, 360f, fov.ViewRadius);

        var viewAngleA = DirectionFromAngle(transform.eulerAngles.y, -fov.ViewAngle / 2);
        var viewAngleB = DirectionFromAngle(transform.eulerAngles.y, fov.ViewAngle / 2);

        Handles.color = Color.yellow;
        Handles.DrawLine(transform.position, transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(transform.position, transform.position + viewAngleB * fov.ViewRadius);

        if (fov.Target != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(transform.position, fov.Target.position);
        }
    }

    private Vector3 DirectionFromAngle(float eulerY, float angleInDegrees)
    {
        angleInDegrees += eulerY;
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
