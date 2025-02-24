using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [field: SerializeField, ReadOnly]
    public bool IsDetected { get; private set; }

    [field: SerializeField, ReadOnly]
    public bool IsInteracted { get; private set; }

    [field: SerializeField]
    public string ObjectName { get; protected set; }

    [field: SerializeField]
    public string ActionName { get; protected set; }

    [field: SerializeField]
    public float HoldTime { get; protected set; }    // 상호작용까지의 시간

    [field: SerializeField]
    public bool CanInteract { get; protected set; } = true;

    [field: SerializeField]
    public Vector3 UIOffset { get; protected set; }

    public void Detected(Interactor interactor)
    {
        if (!IsDetected)
        {
            IsDetected = true;
            OnDetected(interactor);
        }
    }

    public void Undetected(Interactor interactor)
    {
        if (IsDetected)
        {
            IsDetected = false;
            OnUndetected(interactor);
        }
    }

    public void StartInteraction(Interactor interactor)
    {
        if (!IsInteracted)
        {
            IsInteracted = true;
            OnInteracted(interactor);
        }
    }

    public void EndInteraction(Interactor interactor)
    {
        if (IsInteracted)
        {
            IsInteracted = false;
            OnUninteracted(interactor);
        }
    }

    public abstract void OnDetected(Interactor interactor);
    public abstract void OnUndetected(Interactor interactor);
    public abstract void OnInteracted(Interactor interactor);
    public abstract void OnUninteracted(Interactor interactor);

    private void OnDrawGizmosSelected()
    {
        // UIOffset 위치 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + UIOffset, 0.1f);
    }
}
