using UnityEngine;

public class InteractableTest : Interactable
{
    public override void OnDetected(Interactor interactor)
    {
        Debug.Log("Detect");
    }

    public override void OnUndetected(Interactor interactor)
    {
        Debug.Log("Undetect");
    }

    public override void OnInteractionStarted(Interactor interactor)
    {
        Debug.Log("Interact");
    }

    public override void OnInteractionEnded(Interactor interactor)
    {
        Debug.Log("Uninteract");
    }
}
