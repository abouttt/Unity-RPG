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
        InputManager.Enabled = true;
        InputManager.CursorLocked = true;
        SoundManager.Play2D(_bgm, SoundType.BGM);
        UIManager.Get<UI_GlobalCanvas>().Fade(1f, 0f, SceneSettings.Instance.FadeInDuration);
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var itemInventory = player.GetComponent<ItemInventory>();
        var interactor = player.GetComponentInChildren<Interactor>();
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();

        UIManager.Get<UI_ItemInventoryPopup>().Connect(itemInventory);
        UIManager.Get<UI_AutoCanvas>().InteractorUI.Connect(interactor);
        UIManager.Get<UI_AutoCanvas>().LockOnUI.Connect(lockOnFov);
    }
}
