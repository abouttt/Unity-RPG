using UnityEngine;

public class UI_Interactor : UI_View, IConnectable<Interactor>
{
    public Interactor Context => _interactorRef;

    private Interactor _interactorRef;
    private UI_FollowWorldObject _followTarget;

    protected override void Init()
    {
        _followTarget = GetComponent<UI_FollowWorldObject>();
        GetText("KeyText").text = InputManager.GetBindingPath("Interact");
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_interactorRef.Target.IsInteracted)
        {
            GetObject("Body").SetActive(false);
            return;
        }

        if (!GetObject("Body").activeSelf)
        {
            GetObject("Body").SetActive(true);
        }

        if (GetImage("HoldingTimeImage").IsActive())
        {
            GetImage("HoldingTimeImage").fillAmount = _interactorRef.HoldingTime / _interactorRef.Target.HoldTime;
        }
    }

    public void Connect(Interactor interactor)
    {
        Disconnect();

        _interactorRef = interactor;
        interactor.TargetChanged += SetTarget;
    }

    public void Disconnect()
    {
        if (_interactorRef != null)
        {
            _interactorRef.TargetChanged -= SetTarget;
            _interactorRef = null;
        }
    }

    private void SetTarget(Interactable target)
    {
        bool isNotNull = target != null;

        if (isNotNull)
        {
            bool canInteract = target.CanInteract;
            GetImage("BackgroundImage").gameObject.SetActive(canInteract);
            GetText("KeyText").gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.HoldTime > 0f;
            GetImage("HoldingTimeImage").gameObject.SetActive(hasHoldTime);
            GetImage("FrameImage").gameObject.SetActive(hasHoldTime);

            var actionText = GetText("ActionText");
            actionText.text = target.ActionName;
            actionText.gameObject.SetActive(canInteract);

            var name = GetText("NameText");
            name.text = target.ObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
