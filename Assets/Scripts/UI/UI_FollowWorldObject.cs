using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    [field: SerializeField]
    public Transform Target { get; set; }

    private Camera _mainCamera;
    private RectTransform _rt;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (Target != null)
        {
            _rt.position = _mainCamera.WorldToScreenPoint(Target.position);
        }
    }
}
