using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_View
{
    protected override void Init()
    {

    }

    private void Start()
    {
        var bg = _binder.Get<Image>("BG");
        bg.sprite = SceneSettings.Instance[SceneLoader.NextSceneName].LoadingBackground;
        bg.color = bg.sprite != null ? Color.white : Color.black;
        _binder.Get<Image>("Bar").fillAmount = 0f;
    }

    private void Update()
    {
        _binder.Get<Image>("Bar").fillAmount = SceneLoader.Progress;
    }
}
