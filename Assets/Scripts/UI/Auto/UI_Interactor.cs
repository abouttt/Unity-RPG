using UnityEngine;

public class UI_Interactor : UI_View, IConnectable<Interactor>
{
    public Interactor Context => _interactor;

    private Interactor _interactor;

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        UpdateInteractorInfo();
    }

    public void Connect(Interactor interactor)
    {
        Disconnect();

        _interactor = interactor;
        interactor.TargetChanged += SetTarget;
    }

    public void Disconnect()
    {
        if (_interactor != null)
        {
            _interactor.TargetChanged -= SetTarget;
            _interactor = null;
        }
    }

    private void UpdateInteractorInfo()
    {
        if (_interactor.Target.IsInteracted)
        {
            GetObject("Root").SetActive(false);
        }
        else
        {
            GetObject("Root").SetActive(true);

            var holdingTimeBarImage = GetImage("HoldingTimeBarImage");
            if (holdingTimeBarImage.IsActive())
            {
                holdingTimeBarImage.fillAmount = _interactor.HoldingTime / _interactor.Target.HoldTime;
            }
        }
    }

    private void SetTarget(Interactable target)
    {
        if (target != null)
        {
            bool canInteract = target.CanInteract;
            GetImage("BackgroundImage").gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.HoldTime > 0f;
            GetImage("HoldingTimeBarImage").gameObject.SetActive(hasHoldTime);
            GetImage("HoldingTimeFrameImage").gameObject.SetActive(hasHoldTime);

            var keyText = GetText("KeyText");
            keyText.gameObject.SetActive(canInteract);
            keyText.text = Managers.Input.FindBindingPath("Interact");

            var actionText = GetText("ActionText");
            actionText.text = target.ActionName;
            actionText.gameObject.SetActive(canInteract);

            var nameText = GetText("NameText");
            nameText.text = target.ObjectName;
            nameText.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            Get<UI_FollowWorldObject>("Root").Set(target.transform, target.UIOffset);
        }
        else
        {
            Get<UI_FollowWorldObject>("Root").Target = null;
        }

        gameObject.SetActive(target != null);
    }

    private void OnDestroy()
    {
        Disconnect();
    }
}
