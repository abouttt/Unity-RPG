using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    private static bool _initializedManagers;

    private void Awake()
    {
        string sceneName = SceneManagerEx.CurrentSceneName;

        if (ResourceManager.Count == 0 &&
            SceneSettings.Instance[sceneName].ReloadSceneWhenNoResources)
        {
            SceneManagerEx.ReadyToLoad(sceneName);
        }
        else
        {
            Init();
        }
    }

    protected virtual void Init()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            ResourceManager.InstantiateAsync("EventSystem");
        }

        if (!UIManager.Contains<UI_GlobalCanvas>())
        {
            ResourceManager.InstantiateAsync("UI_GlobalCanvas");
        }
    }

    protected void InstantiatePackage(string packageAddress)
    {
        ResourceManager.InstantiateAsync(packageAddress, package =>
        {
            package.transform.DetachChildren();
            Destroy(package);
        });
    }
}
