using UnityEngine;
using UnityEngine.UI;

public class UI_Loading : UI_View
{
    protected override void Init()
    {
        string nextSceneName = SceneLoader.NextSceneName;
        if (nextSceneName != null)
        {
            var bg = Get<Image>("BG");
            bg.sprite = SceneSettings.Instance[nextSceneName].LoadingBackground;
            bg.color = bg.sprite != null ? Color.white : Color.black;
        }
        Get<Image>("Bar").fillAmount = 0f;
    }

    private void Update()
    {
        Get<Image>("Bar").fillAmount = SceneLoader.Progress;
    }
}
