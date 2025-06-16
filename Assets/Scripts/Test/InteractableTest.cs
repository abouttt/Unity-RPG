using UnityEngine;

public class InteractableTest : Interactable
{
    protected override void OnDetected(Interactor interactor)
    {
        Debug.Log("Detect");
    }

    protected override void OnUndetected(Interactor interactor)
    {
        Debug.Log("Undetect");
    }

    protected override void OnInteractionStarted(Interactor interactor)
    {
        Debug.Log("Interact");
    }

    protected override void OnInteractionEnded(Interactor interactor)
    {
        Debug.Log("Uninteract");
    }
}
