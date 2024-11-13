using UnityEngine;

public class GameScene : BaseScene
{
    [field: SerializeField]
    private AudioClip _bgm;

    private void Start()
    {
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play2D(_bgm, SoundType.BGM);
        Managers.UI.Get<UI_GlobalCanvas>().Fade(1f, 0f, SceneSettings.Instance.FadeInDuration);
    }
}
