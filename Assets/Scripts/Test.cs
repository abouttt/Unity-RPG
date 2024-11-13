using UnityEngine;

public class Test : MonoBehaviour
{
    public GameObject Prefab;

    private void Start()
    {
        PoolManager.Instance.CreatePool(Prefab);
    }
}
