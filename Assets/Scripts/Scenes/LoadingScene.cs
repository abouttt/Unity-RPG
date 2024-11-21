using System;
using System.Collections;
using UnityEngine;

public class LoadingScene : BaseScene
{
    [SerializeField]
    private string _defaultSceneName;

    protected override void Init()
    {
        base.Init();

        ClearManagers();
        Resources.UnloadUnusedAssets();
        GC.Collect();

        if (!SceneManagerEx.IsReadyToLoad)
        {
            SceneManagerEx.ReadyToLoad(_defaultSceneName);
        }

        SceneManagerEx.ReadyToLoadCompleted += () => StartCoroutine(LoadComplete());
    }

    private void Start()
    {
        LoadResourcesByLabels(SceneManagerEx.StartLoad);
    }

    private IEnumerator LoadComplete()
    {
        UIManager.Get<UI_GlobalCanvas>().Fade(0f, 1f, SceneSettings.Instance.FadeOutDuration);
        yield return YieldCache.WaitForSeconds(SceneSettings.Instance.FadeOutDuration);
        SceneManagerEx.CompleteLoad();
    }

    private void LoadResourcesByLabels(Action callback = null)
    {
        var loadResourceLabels = SceneSettings.Instance[SceneManagerEx.NextSceneName].AddressableLabels;
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

    private void ClearManagers()
    {
        CooldownManager.Clear();

        InputManager.Clear();
        PoolManager.Clear();
        ResourceManager.Clear();
        SoundManager.Clear();
        UIManager.Clear();
    }
}
