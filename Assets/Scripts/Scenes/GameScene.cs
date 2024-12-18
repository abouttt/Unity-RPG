using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField]
    private AudioClip _bgm;

    protected override void Init()
    {
        base.Init();
        UIManager.CreateUI();
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
        UIManager.Get<UI_LockOn>().Connect(lockOnFov);
    }
}
