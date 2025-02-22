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

    public void Detected()
    {
        if (!IsDetected)
        {
            IsDetected = true;
            OnDetect();
        }
    }

    public void Undetected()
    {
        if (IsDetected)
        {
            IsDetected = false;
            OnUndetect();
        }
    }

    public void StartInteraction()
    {
        if (!IsInteracted)
        {
            IsInteracted = true;
            OnInteract();
        }
    }

    public void EndInteraction()
    {
        if (IsInteracted)
        {
            IsInteracted = false;
            OnUninteract();
        }
    }

    public abstract void OnDetect();
    public abstract void OnUndetect();
    public abstract void OnInteract();
    public abstract void OnUninteract();

    private void OnDrawGizmosSelected()
    {
        // UIOffset 위치 시각화
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position + UIOffset, 0.1f);
    }
}
