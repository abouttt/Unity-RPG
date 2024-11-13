using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingFixed : UI_View
{
    [SerializeField]
    private Image _bg;

    [SerializeField]
    private Image _bar;

    protected override void Init()
    {
        _bg.sprite = SceneSettings.Instance[Managers.Scene.NextSceneName].LoadingBackground;
        _bg.color = _bg.sprite != null ? Color.white : Color.black;

        _bar.fillAmount = 0f;
    }

    private void Update()
    {
        _bar.fillAmount = Managers.Scene.LoadingProgress;
    }
}
