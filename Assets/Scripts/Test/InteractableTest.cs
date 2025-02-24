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

    public override void OnInteracted(Interactor interactor)
    {
        Debug.Log("Interact");
        EndInteraction(interactor);
    }

    public override void OnUninteracted(Interactor interactor)
    {
        Debug.Log("Uninteract");
    }
}
