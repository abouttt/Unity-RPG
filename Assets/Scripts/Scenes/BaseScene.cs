using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    private void Awake()
    {
        string sceneName = Managers.Scene.CurrentSceneName;

        if (Managers.Resource.Count == 0 &&
            SceneSettings.Instance[sceneName].ReloadSceneWhenNoResources)
        {
            Managers.Scene.ReadyToLoad(sceneName);
        }
        else
        {
            Init();
        }
    }

    protected virtual void Init()
    {
        Managers.Init();

        if (FindAnyObjectByType<EventSystem>() == null)
        {
            Managers.Resource.InstantiateAsync("EventSystem");
        }
    }
}
