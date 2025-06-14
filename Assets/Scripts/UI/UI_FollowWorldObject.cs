using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    [field: SerializeField]
    public Transform Target { get; set; }

    [field: SerializeField]
    public Vector3 Offset { get; set; }

    private Camera _mainCamera;
    private RectTransform _rt;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        UpdatePosition();
    }

    public void Set(Transform target, Vector3 offset)
    {
        Target = target;
        Offset = offset;
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (Target != null)
        {
            _rt.position = _mainCamera.WorldToScreenPoint(Target.position + Offset);
        }
    }
}
