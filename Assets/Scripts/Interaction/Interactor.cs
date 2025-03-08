using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public event Action<Interactable> TargetChanged;

    public Interactable Target
    {
        get => _target;
        private set
        {
            if (_target == value)
            {
                return;
            }

            if (_target != null)
            {
                if (_target.IsInteracted)
                {
                    _target.EndInteraction(this);
                }

                _target.Undetect(this);
            }

            _target = value;
            HasTarget = _target != null;
            HoldingTime = 0f;
            _isReadyToInteract = false;

            if (_target != null)
            {
                _target.Detect(this);
            }

            TargetChanged?.Invoke(_target);
        }
    }

    public bool Interact { get; set; }
    public bool HasTarget { get; private set; }
    public float HoldingTime { get; private set; }

    [SerializeField]
    private float _findDelay;

    [SerializeField]
    private Vector3 _center;

    [SerializeField]
    private float _radius;

    [SerializeField]
    private LayerMask _targetLayers;

    [SerializeField]
    private LayerMask _obstacleLayers;

    private Interactable _target;
    private bool _isReadyToInteract;

    private async void Start()
    {
        await FindTargetAwaitable();
    }

    private void Update()
    {
        if (CanInteract())
        {
            InteractTarget();
        }
    }

    private async Awaitable FindTargetAwaitable()
    {
        while (true)
        {
            if (CanFind())
            {
                FindTarget();
            }

            await Awaitable.WaitForSecondsAsync(_findDelay);
        }
    }

    private void FindTarget()
    {
        Interactable finalTarget = null;
        var spherePosition = transform.position + transform.rotation * _center;
        var targets = Physics.OverlapSphere(spherePosition, _radius, _targetLayers);

        Array.Sort(targets, (a, b) =>
        {
            float distA = Vector3.SqrMagnitude(a.transform.position - transform.position);
            float distB = Vector3.SqrMagnitude(b.transform.position - transform.position);
            return distA.CompareTo(distB); // 오름차순 정렬
        });

        foreach (var target in targets)
        {
            if (Physics.Linecast(transform.position, target.transform.position, _obstacleLayers))
            {
                continue;
            }

            finalTarget = target.GetComponent<Interactable>();
            break;
        }

        Target = finalTarget;
    }

    private void InteractTarget()
    {
        if (Interact)
        {
            if (_isReadyToInteract && _target.CanInteract)
            {
                if (HoldingTime <= _target.HoldTime)
                {
                    HoldingTime += Time.deltaTime;
                }
                else
                {
                    Interact = false;
                    _target.StartInteraction(this);
                }
            }
        }
        else
        {
            HoldingTime = 0f;
            _isReadyToInteract = true;
        }
    }

    private bool CanFind()
    {
        if (HasTarget && _target.IsInteracted)
        {
            return false;
        }

        return true;
    }

    private bool CanInteract()
    {
        if (_target == null)
        {
            if (HasTarget)
            {
                Target = null;
            }

            return false;
        }

        if (!_target.gameObject.activeSelf)
        {
            Target = null;
            return false;
        }

        if (_target.IsInteracted)
        {
            return false;
        }

        return true;
    }

    private void OnDrawGizmosSelected()
    {
        var spherePosition = transform.position + transform.rotation * _center;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spherePosition, _radius);
    }
}
