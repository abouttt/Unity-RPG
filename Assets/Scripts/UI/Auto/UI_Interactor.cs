using UnityEngine;

public class UI_Interactor : UI_View, IConnectable<Interactor>
{
    public Interactor Context => _interactorRef;

    private Interactor _interactorRef;

    protected override void Init()
    {
        base.Init();
        GetText("KeyText").text = Managers.Input.FindBindingPath("Interact");
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        var body = GetObject("Body");

        if (_interactorRef.Target.IsInteracted)
        {
            body.SetActive(false);
            return;
        }

        if (!body.activeSelf)
        {
            body.SetActive(true);
        }

        var holdingTimeBarImage = GetImage("HoldingTimeBarImage");
        if (holdingTimeBarImage.IsActive())
        {
            holdingTimeBarImage.fillAmount = _interactorRef.HoldingTime / _interactorRef.Target.HoldTime;
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

            var nameText = GetText("NameText");
            nameText.text = target.ObjectName;
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            Get<UI_FollowWorldObject>("Body").Set(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
