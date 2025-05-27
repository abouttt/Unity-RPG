using UnityEngine;
using UnityEngine.EventSystems;

public abstract class BaseScene : MonoBehaviour
{
    private void Awake()
    {
        string sceneName = Managers.Scene.CurrentSceneName;

        if (Settings.Scene[sceneName].ReloadSceneWhenNoResources &&
            Managers.Resource.ResourceCount == 0)
        {
            Managers.Scene.PrepareLoad(sceneName);
        }
        else
        {
            Initialize();
        }
    }

    protected virtual void Initialize()
    {
        if (FindAnyObjectByType<EventSystem>() == null)
        {
            Managers.Resource.InstantiateAsync("EventSystem");
        }
    }
}
