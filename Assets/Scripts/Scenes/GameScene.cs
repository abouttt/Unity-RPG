using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Init()
    {
        base.Init();
    }

    private void Start()
    {
        InputManager.Enabled = true;
        InputManager.CursorLocked = true;
        SoundManager.Play2D(SceneSettings.Instance[SceneLoader.CurrentSceneName].BGM, SoundType.BGM);
    }
}
