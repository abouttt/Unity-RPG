using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : MonoSingleton<SceneLoader>
{
    public static event Action Loaded;

    public static string CurrentSceneName => SceneManager.GetActiveScene().name;
    public static string NextSceneName => Instance._nextSceneName;
    public static bool IsPrepare => Instance._isPrepare;
    public static bool IsLoading => Instance._isLoading;
    public static bool IsLoaded => Instance._isLoaded;
    public static float Progress => Instance._progress;

    [SerializeField]
    private string _loadingSceneName = "LoadingScene";

    private string _nextSceneName;
    private bool _isPrepare;
    private bool _isLoading;
    private bool _isLoaded;
    private float _progress;
    private AsyncOperation _loadingOp;

    public static void PrepareLoad(string sceneName)
    {
        var instance = Instance;

        if (instance._isLoading || instance._isLoaded)
        {
            Debug.LogWarning($"[SceneLoader] Already in loading or loaded: {instance._nextSceneName}");
            return;
        }

        instance._nextSceneName = sceneName;
        instance._isPrepare = true;

        if (CurrentSceneName != instance._loadingSceneName)
        {
            SceneManager.LoadScene(instance._loadingSceneName);
        }
    }

    public static void StartLoad()
    {
        var instance = Instance;

        if (instance._isLoading)
        {
            Debug.LogWarning($"[SceneLoader] Already in loading: {instance._nextSceneName}");
            return;
        }

        if (instance._isPrepare)
        {
            instance.StartCoroutine(instance.LoadSceneAsync());
        }
        else
        {
            Debug.LogWarning("[SceneLoader] Before you start loading, prepare to load");
        }
    }

    public static void SwitchLoadedScene()
    {
        var instance = Instance;

        if (instance._isLoaded)
        {
            instance.ClearStatus();
            instance._loadingOp.allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("[SceneLoader] Not loaded");
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        _loadingOp = SceneManager.LoadSceneAsync(_nextSceneName, LoadSceneMode.Single);
        _loadingOp.allowSceneActivation = false;
        _isLoading = true;

        float timer = 0f;

        while (!_loadingOp.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (_loadingOp.progress < 0.9f)
            {
                _progress = Mathf.Lerp(_progress, _loadingOp.progress, timer);
                if (_progress >= _loadingOp.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _progress = Mathf.Lerp(_progress, 1f, timer);
                if (_progress >= 1f)
                {
                    _isLoading = false;
                    _isLoaded = true;
                    Loaded?.Invoke();
                    Loaded = null;
                    yield break;
                }
            }
        }
    }

    private void ClearStatus()
    {
        _nextSceneName = null;
        _isPrepare = false;
        _isLoaded = false;
        _progress = 0f;
    }
}
