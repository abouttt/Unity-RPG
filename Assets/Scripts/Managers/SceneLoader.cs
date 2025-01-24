using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class SceneLoader : MonoSingleton<SceneLoader>
{
    public static event Action LoadCompleted;

    public static string NextSceneName => Instance._nextSceneName;
    public static bool IsReadyToLoad => Instance._isReadyToLoad;
    public static bool IsLoading => Instance._isLoading;
    public static bool IsReadyToLoadCompleted => Instance._isReadyToLoadCompleted;
    public static float Progress => Instance._progress;

    [SerializeField]
    private string _loadingSceneName = "LoadingScene";

    private string _nextSceneName;
    private bool _isReadyToLoad;
    private bool _isLoading;
    private bool _isReadyToLoadCompleted;
    private float _progress;
    private AsyncOperation _loadSceneOp;

    public static void ReadyToLoad(string sceneName)
    {
        var instance = Instance;

        if (instance._isLoading || instance._isReadyToLoadCompleted)
        {
            Debug.LogWarning($"[SceneLoader.ReadyToLoad] Already in loading or ready to load complete: {instance._nextSceneName}");
            return;
        }

        instance._nextSceneName = sceneName;
        instance._isReadyToLoad = true;

        if (!SceneManager.GetActiveScene().name.Equals(instance._loadingSceneName))
        {
            SceneManager.LoadScene(instance._loadingSceneName);
        }
    }

    public static void StartLoad()
    {
        var instance = Instance;

        if (instance._isLoading)
        {
            Debug.LogWarning($"[SceneLoader.StartLoad] Already loading : {instance._nextSceneName}");
            return;
        }

        if (instance._isReadyToLoad)
        {
            instance.StartCoroutine(instance.LoadSceneAsync(instance._nextSceneName));
        }
        else
        {
            Debug.LogWarning("[SceneLoader.StartLoad] Not ready to load");
        }
    }

    public static void SwitchLoadedScene()
    {
        var instance = Instance;

        if (instance._isReadyToLoadCompleted)
        {
            instance._loadSceneOp.allowSceneActivation = true;
            instance.ClearStatus();
        }
        else
        {
            Debug.LogWarning("[SceneLoader.CompleteLoad] Not ready to complete");
        }
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        _isLoading = true;

        yield return null;

        _loadSceneOp = SceneManager.LoadSceneAsync(sceneName);
        _loadSceneOp.allowSceneActivation = false;

        float timer = 0f;

        while (!_loadSceneOp.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (_loadSceneOp.progress < 0.9f)
            {
                _progress = Mathf.Lerp(_progress, _loadSceneOp.progress, timer);
                if (_progress >= _loadSceneOp.progress)
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
                    _isReadyToLoadCompleted = true;
                    LoadCompleted?.Invoke();
                    LoadCompleted = null;
                    yield break;
                }
            }
        }
    }

    private void ClearStatus()
    {
        _nextSceneName = null;
        _isReadyToLoad = false;
        _isReadyToLoadCompleted = false;
        _progress = 0f;
        _loadSceneOp = null;
    }
}
