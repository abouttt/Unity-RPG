using UnityEngine;

public class InteractableTest : Interactable
{
    protected override void OnDetected()
    {
        Debug.Log("Detected");
    }

    protected override void OnUndetected()
    {
        Debug.Log("Undetected");
    }

    protected override void OnInteract()
    {
        Debug.Log("Interact");
        StopInteract();
    }

    protected override void OnStopInteract()
    {
        Debug.Log("Stop Interact");
    }
}
