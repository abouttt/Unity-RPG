using UnityEngine;

public class UI_Loading : UI_View
{
    protected override void Init()
    {
        base.Init();

        string nextSceneName = Managers.Scene.NextSceneName;
        if (nextSceneName != null)
        {
            var bg = GetImage("BackgroundImage");
            bg.sprite = Settings.Scene[nextSceneName].LoadingBackground;
            bg.color = bg.sprite != null ? Color.white : Color.black;
        }

        GetImage("BarImage").fillAmount = 0f;
    }

    private void Update()
    {
        GetImage("BarImage").fillAmount = Managers.Scene.Progress;
    }
}
