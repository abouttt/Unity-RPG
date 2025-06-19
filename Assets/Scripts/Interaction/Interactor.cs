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
    public float HoldingTime { get; private set; }
    public bool HasTarget => _target != null;

    [SerializeField]
    private LayerMask _interactorLayer;

    [SerializeField]
    private LayerMask _targetLayer;

    [SerializeField]
    private LayerMask _obstacleLayer;

    private Interactable _target;
    private bool _isReadyToInteract;

    private void Awake()
    {
        Utility.SetIgnoreCollision(_interactorLayer, true);
        Utility.SetIgnoreCollision(_targetLayer, true);
        Utility.SetIgnoreCollision(_interactorLayer, _targetLayer, false);
    }

    private void Update()
    {
        if (CanInteract())
        {
            InteractTarget();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!CanFind())
        {
            return;
        }

        if (!HasTarget)
        {
            Target = other.GetComponent<Interactable>();
        }
        else
        {
            if (_target.gameObject != other.gameObject)
            {
                var targetDistance = Vector3.SqrMagnitude(transform.position - _target.transform.position);
                var otherDistance = Vector3.SqrMagnitude(transform.position - other.transform.position);
                if (otherDistance > targetDistance)
                {
                    return;
                }

                if (Physics.Linecast(transform.position, other.transform.position, _obstacleLayer))
                {
                    return;
                }

                Target = other.GetComponent<Interactable>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!HasTarget)
        {
            return;
        }

        if (_target.gameObject != other.gameObject)
        {
            return;
        }

        Target = null;
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
        if (!HasTarget)
        {
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
}
