using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(UI_FollowWorldObject))]
public class UI_Interactor : MonoBehaviour, IConnectable<Interactor>
{
    [SerializeField]
    private GameObject _body;

    [SerializeField]
    private Image _loadingTimeImage;

    [SerializeField]
    private Image _bg;

    [SerializeField]
    private Image _frame;

    [SerializeField]
    private TextMeshProUGUI _keyText;

    [SerializeField]
    private TextMeshProUGUI _interactionText;

    [SerializeField]
    private TextMeshProUGUI _nameText;

    private Interactor _interactor;
    private UI_FollowWorldObject _followTarget;

    private void Awake()
    {
        _followTarget = GetComponent<UI_FollowWorldObject>();
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_interactor.Target.IsInteracted)
        {
            _body.SetActive(false);
            return;
        }

        if (!_body.activeSelf)
        {
            _body.SetActive(true);
        }

        if (_loadingTimeImage.IsActive())
        {
            _loadingTimeImage.fillAmount = _interactor.HoldingTime / _interactor.Target.HoldTime;
        }
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
            _bg.gameObject.SetActive(canInteract);
            _keyText.gameObject.SetActive(canInteract);

            bool hasHoldTime = canInteract && target.HoldTime > 0f;
            _loadingTimeImage.gameObject.SetActive(hasHoldTime);
            _frame.gameObject.SetActive(hasHoldTime);

            _interactionText.text = target.ActionName;
            _interactionText.gameObject.SetActive(canInteract);

            _nameText.text = target.ObjectName;
            _nameText.gameObject.SetActive(!string.IsNullOrEmpty(target.ObjectName));

            _followTarget.SetTargetAndOffset(target.transform, target.UIOffset);
        }

        gameObject.SetActive(isNotNull);
    }
}
