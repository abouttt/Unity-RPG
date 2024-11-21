using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public sealed class SceneManagerEx : MonoBehaviourSingleton<SceneManagerEx>
{
    public static event Action ReadyToLoadCompleted;

    public static string CurrentSceneName => SceneManager.GetActiveScene().name;
    public static string NextSceneName => Instance._nextSceneName;
    public static bool IsReadyToLoad => Instance._isReadyToLoad;
    public static bool IsLoading => Instance._isLoading;
    public static bool IsReadyToLoadComplete => Instance._isReadyToLoadComplete;
    public static float LoadingProgress => Instance._loadingProgress;

    [SerializeField]
    private string _loadingSceneName = "LoadingScene";

    private string _nextSceneName;
    private bool _isReadyToLoad;
    private bool _isLoading;
    private bool _isReadyToLoadComplete;
    private float _loadingProgress;
    private AsyncOperation _loadSceneOp;

    protected override void Init()
    {
        base.Init();
        Addressables.InitializeAsync();
    }

    protected override void Dispose()
    {
        base.Dispose();
        ClearStatus();
        ReadyToLoadCompleted = null;
    }

    public static void ReadyToLoad(string sceneName)
    {
        var instance = Instance;

        if (instance._isLoading || instance._isReadyToLoadComplete)
        {
            Debug.LogWarning($"[SceneManagerEx.ReadyToLoad] Already in loading or ready to complete: {instance._nextSceneName}");
            return;
        }

        instance._nextSceneName = sceneName;
        instance._isReadyToLoad = true;

        if (!CurrentSceneName.Equals(instance._loadingSceneName))
        {
            SceneManager.LoadScene(instance._loadingSceneName);
        }
    }

    public static void StartLoad()
    {
        var instance = Instance;

        if (IsLoading)
        {
            Debug.LogWarning($"[SceneManagerEx.StartLoad] Already loading : {instance._nextSceneName}");
            return;
        }

        if (instance._isReadyToLoad)
        {
            instance._isLoading = true;
            instance._loadSceneOp = SceneManager.LoadSceneAsync(instance._nextSceneName);
            instance._loadSceneOp.allowSceneActivation = false;
            instance.StartCoroutine(LoadSceneAsync(instance._loadSceneOp));
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx.StartLoad] Not ready to load");
        }
    }

    public static void CompleteLoad()
    {
        var instance = Instance;

        if (instance._isReadyToLoadComplete)
        {
            instance.ClearStatus();
            instance._loadSceneOp.allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx.CompleteLoad] Not ready to complete");
        }
    }

    private static IEnumerator LoadSceneAsync(AsyncOperation op)
    {
        yield return null;

        var instance = Instance;
        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                instance._loadingProgress = Mathf.Lerp(instance._loadingProgress, op.progress, timer);
                if (instance._loadingProgress >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                instance._loadingProgress = Mathf.Lerp(instance._loadingProgress, 1f, timer);
                if (instance._loadingProgress >= 1f)
                {
                    instance._isLoading = false;
                    instance._isReadyToLoadComplete = true;
                    ReadyToLoadCompleted?.Invoke();
                    ReadyToLoadCompleted = null;
                    yield break;
                }
            }
        }
    }

    private void ClearStatus()
    {
        _nextSceneName = null;
        _isReadyToLoad = false;
        _isReadyToLoadComplete = false;
        _loadingProgress = 0f;
    }
}
