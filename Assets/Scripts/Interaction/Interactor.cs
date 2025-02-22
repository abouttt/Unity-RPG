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
                _target.Undetected();
            }

            _target = value;
            HasTarget = _target != null;
            HoldingTime = 0f;
            _isReadyToInteract = false;
            _isTargetOutOfRange = false;

            if (_target != null)
            {
                _target.Detected();
            }

            TargetChanged?.Invoke(_target);
        }
    }

    public bool HasTarget { get; private set; }
    public float HoldingTime { get; private set; }
    public bool Interact { get; set; }

    [SerializeField]
    private LayerMask _targetLayers;

    [SerializeField]
    private LayerMask _obstacleLayers;

    private Interactable _target;
    private bool _isReadyToInteract;
    private bool _isTargetOutOfRange;

    private void Awake()
    {
        gameObject.AllIgnoreLayerCollision();
        gameObject.SetLayerCollision(_targetLayers, false);
    }

    private void Update()
    {
        if (CanInteract())
        {
            InteractTarget();
        }
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

        if (_isTargetOutOfRange)
        {
            Target = null;
            return false;
        }

        return true;
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
                    HoldingTime = 0f;
                    _target.StartInteraction();
                }
            }
        }
        else
        {
            HoldingTime = 0f;
            _isReadyToInteract = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        DetectTarget(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        UndetectTarget(other.gameObject);
    }

    private void DetectTarget(GameObject other)
    {
        if (!other.TryGetComponent<Interactable>(out var interactable))
        {
            return;
        }

        if (_target == null)
        {
            Target = interactable;
        }
        else
        {
            if (_target.IsInteracted)
            {
                return;
            }

            if (_target.gameObject != gameObject)
            {
                float distanceToTarget = Vector3.Distance(transform.position, _target.transform.position);
                float distanceToOther = Vector3.Distance(transform.position, other.transform.position);
                if (distanceToTarget <= distanceToOther)
                {
                    return;
                }

                if (Physics.Linecast(transform.position, other.transform.position, _obstacleLayers))
                {
                    return;
                }

                Target = interactable;
            }
        }
    }

    private void UndetectTarget(GameObject other)
    {
        if (_target.gameObject != other)
        {
            return;
        }

        if (_target.IsInteracted)
        {
            _isTargetOutOfRange = true;
        }
        else
        {
            Target = null;
        }
    }
}
