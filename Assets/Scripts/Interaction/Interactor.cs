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
            if (_target != null)
            {
                _target.Undetected();
            }

            _target = value;
            _hasTarget = _target != null;
            _holdingTime = 0f;
            _isReadyToInteract = false;
            _isTargetOutOfRange = false;

            if (_target != null)
            {
                _target.Detected();
            }

            TargetChanged?.Invoke(_target);
        }
    }

    public bool HasTarget => _hasTarget;
    public float HoldingTime => _holdingTime;
    public bool Interact { get; set; }

    [SerializeField]
    private LayerMask _targetLayers;

    [SerializeField]
    private LayerMask _obstacleLayers;

    private Interactable _target;
    private bool _hasTarget;
    private float _holdingTime;
    private bool _isReadyToInteract;
    private bool _isTargetOutOfRange;

    private void Update()
    {
        if (_target == null)
        {
            if (_hasTarget)
            {
                Target = null;
            }

            return;
        }

        if (!_target.gameObject.activeSelf)
        {
            Target = null;
            return;
        }

        if (_target.IsInteracted)
        {
            return;
        }

        if (_isTargetOutOfRange)
        {
            Target = null;
            return;
        }

        if (Interact)
        {
            if (_isReadyToInteract && _target.CanInteract)
            {
                if (_holdingTime < _target.HoldTime)
                {
                    _holdingTime += Time.deltaTime;
                }
                else
                {
                    _holdingTime = 0f;
                    _target.Interact();
                }
            }
        }
        else
        {
            _holdingTime = 0f;
            _isReadyToInteract = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!other.gameObject.IsInLayerMask(_targetLayers))
        {
            return;
        }

        if (_target == null)
        {
            Target = other.GetComponent<Interactable>();
        }
        else
        {
            if (_target.IsInteracted)
            {
                return;
            }

            if (_target.gameObject != other.gameObject)
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

                Target = other.GetComponent<Interactable>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.gameObject.IsInLayerMask(_targetLayers))
        {
            return;
        }

        if (_target.gameObject != other.gameObject)
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
