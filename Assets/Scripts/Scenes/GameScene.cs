using UnityEngine;
using DG.Tweening;

public class GameScene : BaseScene
{
    [field: SerializeField]
    private AudioClip _bgm;

    protected override void Init()
    {
        base.Init();
        DOTween.Init();
        InstantiatePackage("GameUIPackage");
        ConnectUI();
    }

    private void Start()
    {
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play2D(_bgm, SoundType.BGM);
        Managers.UI.Get<UI_GlobalCanvas>().Fade(1f, 0f, SceneSettings.Instance.FadeInDuration);
    }

    private void OnDestroy()
    {
        DeconnectUI();
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();

        Managers.UI.Get<UI_AutoCanvas>().LockOnUI.Connect(lockOnFov);
    }

    private void DeconnectUI()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        if (Managers.UI.Count == 0)
        {
            return;
        }

        Managers.UI.Get<UI_AutoCanvas>().LockOnUI.Disconnect();
    }
}
