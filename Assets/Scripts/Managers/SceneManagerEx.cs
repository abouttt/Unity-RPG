using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public sealed class SceneManagerEx : MonoBehaviourSingleton<SceneManagerEx>
{
    public event Action ReadyToLoadComplete;

    public string CurrentSceneName => SceneManager.GetActiveScene().name;
    public string NextSceneName => _nextSceneName;
    public bool IsReadyToLoad => _isReadyToLoad;
    public bool IsLoading => _isLoading;
    public bool IsReadyToLoadComplete => _isReadyToLoadComplete;
    public float LoadingProgress => _loadingProgress;

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
        ReadyToLoadComplete = null;
    }

    public void ReadyToLoad(string sceneName)
    {
        if (IsLoading || _isReadyToLoadComplete)
        {
            Debug.LogWarning($"[SceneManagerEx.ReadyToLoad] Already in loading or ready to complete: {_nextSceneName}");
            return;
        }

        _nextSceneName = sceneName;
        _isReadyToLoad = true;

        if (!CurrentSceneName.Equals(_loadingSceneName))
        {
            SceneManager.LoadScene(_loadingSceneName);
        }
    }

    public void StartLoad()
    {
        if (IsLoading)
        {
            Debug.LogWarning($"[SceneManagerEx.StartLoad] Already loading : {_nextSceneName}");
            return;
        }

        if (_isReadyToLoad)
        {
            _isLoading = true;
            _loadSceneOp = SceneManager.LoadSceneAsync(_nextSceneName);
            _loadSceneOp.allowSceneActivation = false;
            StartCoroutine(LoadSceneAsync(_loadSceneOp));
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx.StartLoad] Not ready to load");
        }
    }

    public void CompleteLoad()
    {
        if (_isReadyToLoadComplete)
        {
            ClearStatus();
            _loadSceneOp.allowSceneActivation = true;
        }
        else
        {
            Debug.LogWarning("[SceneManagerEx.CompleteLoad] Not ready to complete");
        }
    }

    private IEnumerator LoadSceneAsync(AsyncOperation op)
    {
        yield return null;

        float timer = 0f;

        while (!op.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if (op.progress < 0.9f)
            {
                _loadingProgress = Mathf.Lerp(_loadingProgress, op.progress, timer);
                if (_loadingProgress >= op.progress)
                {
                    timer = 0f;
                }
            }
            else
            {
                _loadingProgress = Mathf.Lerp(_loadingProgress, 1f, timer);
                if (_loadingProgress >= 1f)
                {
                    _isLoading = false;
                    _isReadyToLoadComplete = true;
                    ReadyToLoadComplete?.Invoke();
                    ReadyToLoadComplete = null;
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
