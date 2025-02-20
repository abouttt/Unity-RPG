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
        SoundManager.Play2D(SceneSettings.Instance[SceneLoader.CurrentSceneName].BGM, SoundType.BGM);
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();
        UIManager.Get<UI_LockOn>().Connect(lockOnFov);
    }
}
