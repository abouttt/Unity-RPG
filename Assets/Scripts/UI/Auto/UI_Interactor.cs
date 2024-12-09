using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : MonoBehaviour, IConnectable<Interactor>
{
    private DataBinder _binder;
    private Interactor _interactor;
    private UI_FollowWorldObject _followTarget;

    private void Awake()
    {
        _binder = new(gameObject);
        _followTarget = GetComponent<UI_FollowWorldObject>();
        _binder.GetText("KeyText").text = InputManager.GetBindingPath("Interact");

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_interactor.Target.IsInteracted)
        {
            _binder.GetObject("Body").SetActive(false);
            return;
        }

        if (!_binder.GetObject("Body").activeSelf)
        {
            _binder.GetObject("Body").SetActive(true);
        }

        if (_binder.GetImage("LoadingTimeImage").IsActive())
        {
            _binder.GetImage("LoadingTimeImage").fillAmount = _interactor.HoldingTime / _interactor.Target.HoldTime;
        }
    }

    private void OnDestroy()
    {
        Disconnect();
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

    private void SetTarget(Interactable target)
    {
        bool isNotNull = target != null;

        if (isNotNull)
        {
            bool canInteract = target.CanInteract;
            _binder.GetImage("BG").gameObject.SetActive(canInteract);
            _binder.GetText("KeyText").gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.HoldTime > 0f;
            _binder.GetImage("LoadingTimeImage").gameObject.SetActive(hasHoldTime);
            _binder.GetImage("Frame").gameObject.SetActive(hasHoldTime);

            var interactionText = _binder.GetText("InteractionText");
            interactionText.text = target.ActionName;
            interactionText.gameObject.SetActive(canInteract);

            var name = _binder.GetText("NameText");
            name.text = target.ObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
