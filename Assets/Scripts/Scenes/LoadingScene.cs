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
        Managers.Clear();
        Resources.UnloadUnusedAssets();
        GC.Collect();

        if (!Managers.Scene.IsReadyToLoad)
        {
            Managers.Scene.ReadyToLoad(_defaultSceneName);
        }

        Managers.Scene.ReadyToLoadCompleted += () => StartCoroutine(LoadComplete());
    }

    private void Start()
    {
        LoadResourcesByLabels(Managers.Scene.StartLoad);
    }

    private IEnumerator LoadComplete()
    {
        Managers.UI.Get<UI_GlobalCanvas>().Fade(0f, 1f, SceneSettings.Instance.FadeOutDuration);
        yield return YieldCache.WaitForSeconds(SceneSettings.Instance.FadeOutDuration);
        Managers.Scene.CompleteLoad();
    }

    private void LoadResourcesByLabels(Action callback = null)
    {
        var loadResourceLabels = SceneSettings.Instance[Managers.Scene.NextSceneName].AddressableLabels;
        if (loadResourceLabels == null || loadResourceLabels.Length == 0)
        {
            callback?.Invoke();
            return;
        }

        int totalCount = loadResourceLabels.Length;
        int loadedCount = 0;

        foreach (var label in loadResourceLabels)
        {
            Managers.Resource.LoadAllAsync(label.labelString, _ =>
            {
                if (++loadedCount == totalCount)
                {
                    callback?.Invoke();
                }
            });
        }
    }
}
