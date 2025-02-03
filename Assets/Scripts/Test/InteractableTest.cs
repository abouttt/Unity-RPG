using UnityEngine;

public class InteractableTest : Interactable
{
    public override void OnDetect()
    {
        Debug.Log("Detected");
    }

    public override void OnInteract()
    {
        Debug.Log("Interacted");
        EndInteraction();
    }

    public override void OnUndetect()
    {
        Debug.Log("Undetect");
    }

    public override void OnUninteract()
    {
        Debug.Log("Uninteract");
    }
}
