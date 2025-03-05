using UnityEngine;

public class UI_FollowWorldObject : MonoBehaviour
{
    public Transform Target
    {
        get => _target;
        set => _target = value;
    }

    public Vector3 Offset
    {
        get => _offset;
        set => _offset = value;
    }

    [SerializeField]
    private Transform _target;

    [SerializeField]
    private Vector3 _offset;

    private Camera _mainCamera;
    private RectTransform _rt;

    private void Awake()
    {
        _mainCamera = Camera.main;
        _rt = GetComponent<RectTransform>();
    }

    private void LateUpdate()
    {
        if (_target != null)
        {
            _rt.position = _mainCamera.WorldToScreenPoint(_target.position + _offset);
        }
    }

    public void Set(Transform target, Vector3 offset)
    {
        _target = target;
        _offset = offset;
    }
}
