using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField]
    private AudioClip _bgm;

    protected override void Init()
    {
        base.Init();
        UIManager.InitUISettings();
        ConnectUI();
    }

    private void Start()
    {
        InputManager.Enabled = true;
        InputManager.CursorLocked = true;
        SoundManager.Play2D(_bgm, SoundType.BGM);
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();
        var interactor = player.GetComponentInChildren<Interactor>();
        var itemInventory = player.GetComponent<ItemInventory>();
        UIManager.Get<UI_LockOn>().Connect(lockOnFov);
        UIManager.Get<UI_Interactor>().Connect(interactor);
        UIManager.Get<UI_ItemInventory>().Connect(itemInventory);
    }
}
