using System;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public event Action<Interactable> TargetChanged;

    public Interactable Target { get; private set; }
    public bool HasTarget { get; private set; }
    public float HoldingTime { get; private set; }
    public bool Interact { get; set; }

    [SerializeField]
    private Vector3 _center;

    [SerializeField]
    private float _radius;

    [SerializeField]
    private LayerMask _targetLayers;

    [SerializeField]
    private LayerMask _obstacleLayers;

    private bool _isReadyToInteract;

    private void Update()
    {
        DetectTarget();

        if (CanInteract())
        {
            InteractTarget();
        }
    }

    private void DetectTarget()
    {
        if (HasTarget && Target.IsInteracted)
        {
            return;
        }

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

        SetTarget(finalTarget);
    }

    private bool CanInteract()
    {
        return HasTarget && !Target.IsInteracted;
    }

    private void InteractTarget()
    {
        if (Interact)
        {
            if (_isReadyToInteract && Target.CanInteract)
            {
                if (HoldingTime <= Target.HoldTime)
                {
                    HoldingTime += Time.deltaTime;
                }
                else
                {
                    Interact = false;
                    Target.StartInteraction(this);
                }
            }
        }
        else
        {
            HoldingTime = 0f;
            _isReadyToInteract = true;
        }
    }

    private void SetTarget(Interactable target)
    {
        if (Target == target)
        {
            return;
        }

        if (Target != null)
        {
            Target.Undetected(this);
        }

        Target = target;
        HasTarget = target != null;
        HoldingTime = 0f;
        _isReadyToInteract = false;

        if (target != null)
        {
            target.Detected(this);
        }

        TargetChanged?.Invoke(target);
    }

    private void OnDrawGizmosSelected()
    {
        var spherePosition = transform.position + transform.rotation * _center;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(spherePosition, _radius);
    }
}
