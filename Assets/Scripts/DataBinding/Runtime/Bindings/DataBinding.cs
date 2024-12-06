using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class DataBinding : MonoBehaviour
{
    [ReadOnly]
    public Object Target;
    public string DataID;
    public bool AutoRefreshDataID = true;

    public abstract Type BindingType { get; }

    private void Reset()
    {
        DataID = gameObject.name;
        Setup();
    }

    protected abstract void Setup();

    protected void FindTarget<T>() where T : Component
    {
        if (TryGetComponent<T>(out var component))
        {
            Target = component;
        }
        else
        {
            Debug.LogError($"[{BindingType}Binding] Required component {typeof(T).Name} is missing on {gameObject.name}");
        }
    }
}
