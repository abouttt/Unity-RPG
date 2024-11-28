using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : MonoBehaviour, IConnectable<Interactor>
{
    enum Objects
    {
        Body,
    }

    enum Images
    {
        LoadingTimeImage,
        BG,
        Frame,
    }

    enum Texts
    {
        KeyText,
        InteractionText,
        NameText,
    }

    private Interactor _interactor;
    private UI_FollowWorldObject _followTarget;

    private UIBinder _binder;

    private void Awake()
    {
        _binder = new(gameObject);
        _binder.BindObject(typeof(Objects));
        _binder.BindImage(typeof(Images));
        _binder.BindText(typeof(Texts));

        _followTarget = GetComponent<UI_FollowWorldObject>();
        _binder.GetText((int)Texts.KeyText).text = InputManager.GetBindingPath("Interact");

        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_interactor.Target.IsInteracted)
        {
            _binder.GetObject((int)Objects.Body).SetActive(false);
            return;
        }

        if (!_binder.GetObject((int)Objects.Body).activeSelf)
        {
            _binder.GetObject((int)Objects.Body).SetActive(true);
        }

        if (_binder.GetImage((int)Images.LoadingTimeImage).IsActive())
        {
            _binder.GetImage((int)Images.LoadingTimeImage).fillAmount = _interactor.HoldingTime / _interactor.Target.HoldTime;
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
            _binder.GetImage((int)Images.BG).gameObject.SetActive(canInteract);
            _binder.GetText((int)Texts.KeyText).gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.HoldTime > 0f;
            _binder.GetImage((int)Images.LoadingTimeImage).gameObject.SetActive(hasHoldTime);
            _binder.GetImage((int)Images.Frame).gameObject.SetActive(hasHoldTime);

            var interactionText = _binder.GetText((int)Texts.InteractionText);
            interactionText.text = target.ActionName;
            interactionText.gameObject.SetActive(canInteract);

            var name = _binder.GetText((int)Texts.NameText);
            name.text = target.ObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
