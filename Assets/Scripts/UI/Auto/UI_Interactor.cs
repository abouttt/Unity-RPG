using UnityEngine;

public class UI_Interactor : UI_View, IConnectable<Interactor>
{
    public Interactor Context => _interactorRef;

    private Interactor _interactorRef;

    protected override void Init()
    {
        base.Init();
        GetText("KeyText").text = InputManager.GetBindingPath("Interact");
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_interactorRef.Target.IsInteracted)
        {
            GetObject("Interactor").SetActive(false);
            return;
        }

        if (!GetObject("Interactor").activeSelf)
        {
            GetObject("Interactor").SetActive(true);
        }

        if (GetImage("HoldingTimeBarImage").IsActive())
        {
            GetImage("HoldingTimeBarImage").fillAmount = _interactorRef.HoldingTime / _interactorRef.Target.HoldTime;
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
            GetImage("HoldingTimeBarImage").gameObject.SetActive(hasHoldTime);
            GetImage("HoldingTimeFrameImage").gameObject.SetActive(hasHoldTime);

            var actionText = GetText("ActionText");
            actionText.text = target.ActionName;
            actionText.gameObject.SetActive(canInteract);

            var name = GetText("NameText");
            name.text = target.ObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            Get<UI_FollowWorldObject>("Interactor").SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
