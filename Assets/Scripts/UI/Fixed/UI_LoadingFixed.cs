using UnityEngine;
using UnityEngine.UI;

public class UI_LoadingFixed : UI_View
{
    private DataBinder _binder;

    protected override void Init()
    {
        _binder = new(gameObject);

        var bg = _binder.Get<Image>("BG");
        bg.sprite = SceneSettings.Instance[SceneManagerEx.NextSceneName].LoadingBackground;
        bg.color = bg.sprite != null ? Color.white : Color.black;
        _binder.Get<Image>("Bar").fillAmount = 0f;
    }

    private void Update()
    {
        _binder.Get<Image>("Bar").fillAmount = SceneManagerEx.LoadingProgress;
    }
}
