using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FieldOfView))]
public class FieldOfViewEditor : Editor
{
    private void OnSceneGUI()
    {
        var fov = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fov.transform.position, Vector3.up, Vector3.forward, 360f, fov.ViewRadius);

        Vector3 viewAngleA = DirectionFromAngle(fov.ViewAngle * 0.5f);
        Vector3 viewAngleB = DirectionFromAngle(-fov.ViewAngle * 0.5f);
        Handles.color = Color.yellow;
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleA * fov.ViewRadius);
        Handles.DrawLine(fov.transform.position, fov.transform.position + viewAngleB * fov.ViewRadius);

        if (fov.Target != null)
        {
            Handles.color = Color.red;
            Handles.DrawLine(fov.transform.position, fov.Target.position);
        }
    }

    private Vector3 DirectionFromAngle(float angleInDegrees)
    {
        return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0f, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
    }
}
