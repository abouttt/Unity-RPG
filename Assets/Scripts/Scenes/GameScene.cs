using System;
using UnityEngine;

public class GameScene : BaseScene
{
    protected override void Initialize()
    {
        base.Initialize();
        InstantiateUIFromSettings();
        ConnectUI();
    }

    private void Start()
    {
        Managers.Input.Enabled = true;
        Managers.Input.CursorLocked = true;
        Managers.Sound.Play2D(Settings.Scene[Managers.Scene.CurrentSceneName].BGM, SoundType.BGM);
    }

    private void InstantiateUIFromSettings()
    {
        foreach (UIType uiType in Enum.GetValues(typeof(UIType)))
        {
            foreach (var prefab in Settings.UI[uiType].Prefabs)
            {
                var uiGameObject = Instantiate(prefab);
                var view = uiGameObject.GetComponent<UI_View>();
                Managers.UI.Add(view);
            }
        }
    }

    private void ConnectUI()
    {
        var player = GameObject.FindWithTag("Player");
        var lockOnFov = Camera.main.GetComponent<FieldOfView>();
        Managers.UI.Get<UI_LockOn>().Connect(lockOnFov);
    }
}
