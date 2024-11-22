using UnityEngine;
using UnityEngine.UI;

public class UI_CooldownImage : MonoBehaviour, IConnectable<Cooldown>
{
    private Cooldown _cooldown;
    private Image _cooldownImage;

    private void Awake()
    {
        _cooldownImage = GetComponent<Image>();
    }

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        if (_cooldown.RemainingTime > 0f)
        {
            _cooldownImage.fillAmount = _cooldown.RemainingTime / _cooldown.MaxTime;
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        Disconnect();
    }

    public void Connect(Cooldown cooldown)
    {
        if (cooldown == null)
        {
            return;
        }

        Disconnect();

        _cooldown = cooldown;
        cooldown.CooldownStarted += Show;
        if (cooldown.RemainingTime > 0f)
        {
            gameObject.SetActive(true);
        }
    }

    public void Disconnect()
    {
        if (_cooldown != null)
        {
            _cooldown.CooldownStarted -= Show;
            _cooldown = null;
            gameObject.SetActive(false);
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
