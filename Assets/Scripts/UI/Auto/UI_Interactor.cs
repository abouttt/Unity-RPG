using UnityEngine;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : UI_Auto
{
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

    protected override void Init()
    {
        BindImage(typeof(Images));
        BindText(typeof(Texts));

        _interactor = GameObject.FindWithTag("Player").GetComponentInChildren<Interactor>();
        _followTarget = GetComponent<UI_FollowWorldObject>();
    }

    protected override void Start()
    {
        base.Start();
        GetText((int)Texts.KeyText).text = Managers.Input.GetBindingPath("Interact");
        _interactor.TargetChanged += target => SetTarget(target);
    }

    private void LateUpdate()
    {
        if (_interactor.Target == null)
        {
            return;
        }

        if (_interactor.Target.IsInteracted)
        {
            _body.SetActive(false);
            return;
        }

        if (!_body.activeSelf)
        {
            _body.SetActive(true);
        }

        if (GetImage((int)Images.LoadingTimeImage).IsActive())
        {
            GetImage((int)Images.LoadingTimeImage).fillAmount = _interactor.InteractLoadingTime / _interactor.Target.MaxLoadingTime;
        }
    }

    private void SetTarget(Interactable newTarget)
    {
        bool isNotNull = newTarget != null;

        if (isNotNull)
        {
            _followTarget.SetTargetAndOffset(newTarget.transform, newTarget.UIOffset);

            bool canInteract = newTarget.CanInteract;
            GetImage((int)Images.BG).gameObject.SetActive(canInteract);
            GetText((int)Texts.KeyText).gameObject.SetActive(canInteract);

            bool hasLoadingTime = canInteract && newTarget.MaxLoadingTime > 0f;
            GetImage((int)Images.LoadingTimeImage).gameObject.SetActive(hasLoadingTime);
            GetImage((int)Images.Frame).gameObject.SetActive(hasLoadingTime);

            var interactionText = GetText((int)Texts.InteractionText);
            interactionText.text = newTarget.InteractionMessage;
            interactionText.gameObject.SetActive(canInteract);

            var name = GetText((int)Texts.NameText);
            name.text = newTarget.InteractionObjectName;
            name.gameObject.SetActive(!string.IsNullOrEmpty(newTarget.InteractionObjectName));
        }

        _body.SetActive(isNotNull);
    }
}
