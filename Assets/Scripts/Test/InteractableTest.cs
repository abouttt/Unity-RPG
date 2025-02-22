using UnityEngine;

public class InteractableTest : Interactable
{
    public override void OnDetect()
    {
        Debug.Log("Detect");
    }

    public override void OnUndetect()
    {
        Debug.Log("Undetect");
    }

    public override void OnInteract()
    {
        EndInteraction();
        Debug.Log("Interact");
    }

    public override void OnUninteract()
    {
        Debug.Log("Uninteract");
    }
}
