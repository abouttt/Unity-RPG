using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    [field: SerializeField, ReadOnly]
    public bool IsDetected { get; private set; }    // 감지 되었는지

    [field: SerializeField, ReadOnly]
    public bool IsInteracted { get; private set; }  // 상호작용 중인지

    [field: SerializeField]
    public string InteractionObjectName { get; protected set; }

    [field: SerializeField]
    public string InteractionMessage { get; protected set; }

    [field: SerializeField]
    public Vector3 UIOffset { get; protected set; }

    [field: SerializeField]
    public float MaxLoadingTime { get; protected set; }    // 상호작용까지의 시간

    [field: SerializeField]
    public bool CanInteract { get; protected set; } = true;

    public void Detected()
    {
        IsDetected = true;
        OnDetected();
    }

    public void Undetected()
    {
        IsDetected = false;
        OnUndetected();
    }

    public void Interact()
    {
        IsInteracted = true;
        OnInteract();
    }

    public void StopInteract()
    {
        IsInteracted = false;
        OnStopInteract();
    }

    protected virtual void OnDetected() { }
    protected virtual void OnUndetected() { }
    protected virtual void OnInteract() { }
    protected virtual void OnStopInteract() { }

    // InteractionKeyGuidePos 위치 시각화
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + UIOffset, 0.1f);
    }
}
