using System;
using System.Collections;
using UnityEngine;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private string _defaultSceneName;

    private static bool _isInitialized;

    protected override void Init()
    {
        base.Init();

        InitOrCleanup();

        if (!SceneLoader.IsPrepare)
        {
            SceneLoader.PrepareLoad(_defaultSceneName);
        }

        SceneLoader.Loaded += () => StartCoroutine(SwitchLoadedScene());
    }

    private void Start()
    {
        LoadResourcesByLabels(SceneLoader.StartLoad);
    }

    private IEnumerator SwitchLoadedScene()
    {
        yield return YieldCache.WaitForSeconds(Settings.Scene.FadeOutDuration);
        SceneLoader.SwitchLoadedScene();
    }

    private void LoadResourcesByLabels(Action callback = null)
    {
        var loadResourceLabels = Settings.Scene[SceneLoader.NextSceneName].AddressableLabels;
        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            callback?.Invoke();
            return;
        }

        int totalCount = loadResourceLabels.Length;
        int loadedCount = 0;

        foreach (var label in loadResourceLabels)
        {
            ResourceManager.LoadAllAsync(label.labelString, _ =>
            {
                if (++loadedCount == totalCount)
                {
                    callback?.Invoke();
                }
            });
        }
    }

    private void InitOrCleanup()
    {
        if (_isInitialized)
        {
            InputManager.Clear();
            PoolManager.Clear();
            ResourceManager.Clear();
            SoundManager.Clear();
            UIManager.Clear();
            Resources.UnloadUnusedAssets();
            GC.Collect();
        }
        else
        {
            _ = Settings.Scene;
            _ = Settings.UI;
            _ = InputManager.Instance;
            _ = PoolManager.Instance;
            _ = ResourceManager.Instance;
            _ = SceneLoader.Instance;
            _ = SoundManager.Instance;
            _ = UIManager.Instance;

            _isInitialized = true;
        }
    }
}
