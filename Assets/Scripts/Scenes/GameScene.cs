using UnityEngine;

public class GameScene : BaseScene
{
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
        SoundManager.Play2D(Settings.Scene[SceneLoader.CurrentSceneName].BGM, SoundType.BGM);
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();
        var interactor = player.GetComponent<Interactor>();
        UIManager.Get<UI_LockOn>().Connect(lockOnFov);
        UIManager.Get<UI_Interactor>().Connect(interactor);
    }
}
