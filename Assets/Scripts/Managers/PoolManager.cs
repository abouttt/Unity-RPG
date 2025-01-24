using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public sealed class PoolManager : MonoSingleton<PoolManager>
{
    #region Pool

    [Serializable]
    private class Pool
    {
        public Transform Root => _root;
        public int Count => _count;

        [SerializeField, ReadOnly]
        private GameObject _prefab;

        [SerializeField, ReadOnly]
        private int _count;

        private readonly Transform _root;
        private readonly HashSet<GameObject> _activeObjects = new();
        private readonly Queue<GameObject> _inactiveObjects = new();

        public Pool(GameObject prefab, int count)
        {
            _prefab = prefab;
            _count = count;
            _root = new GameObject($"{prefab}_Root").transform;

            for (int i = 0; i < count; i++)
            {
                Create();
            }
        }

        public GameObject Get(Transform parent)
        {
            if (_inactiveObjects.Count == 0)
            {
                Create();
            }

            var go = _inactiveObjects.Dequeue();
            if (parent != null)
            {
                go.transform.SetParent(parent);
            }
            go.SetActive(true);
            _activeObjects.Add(go);

            return go;
        }

        public bool Release(GameObject go)
        {
            if (!_activeObjects.Remove(go))
            {
                return false;
            }

            ChangeInactive(go);

            return true;
        }

        public void ReleaseAll()
        {
            foreach (var go in _activeObjects)
            {
                ChangeInactive(go);
            }

            _activeObjects.Clear();
        }

        public void Clear()
        {
            foreach (var go in _activeObjects)
            {
                Destroy(go);
            }

            foreach (var go in _inactiveObjects)
            {
                Destroy(go);
            }

            _activeObjects.Clear();
            _inactiveObjects.Clear();
            _count = 0;
        }

        public void Dispose()
        {
            Clear();
            Destroy(_root.gameObject);
        }

        private void Create()
        {
            var go = Instantiate(_prefab, _root);
            go.name = _prefab.name;
            ChangeInactive(go);
            _count++;
        }

        private void ChangeInactive(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(_root);
            _inactiveObjects.Enqueue(go);
        }
    }

    #endregion

    [SerializeField, ReadOnly, SerializedDictionary("Name", "Pool")]
    private SerializedDictionary<string, Pool> _pools = new();

    public static void CreatePool(GameObject prefab, int count = 5)
    {
        var instance = Instance;
        var name = prefab.name;

        if (instance._pools.ContainsKey(name))
        {
            Debug.LogWarning($"[PoolManager] Pool for {name} already exist.");
            return;
        }

        var pool = new Pool(prefab, count);
        pool.Root.SetParent(instance.transform);
        instance._pools.Add(name, pool);
    }

    public static GameObject Get(GameObject go, Transform parent = null)
    {
        var instance = Instance;

        if (!instance._pools.TryGetValue(go.name, out var pool))
        {
            CreatePool(go);
            pool = instance._pools[go.name];
        }

        return pool.Get(parent);
    }

    public static GameObject Get(string name, Transform parent = null)
    {
        if (Instance._pools.TryGetValue(name, out var pool))
        {
            return pool.Get(parent);
        }
        else
        {
            return null;
        }
    }

    public static bool Release(GameObject go)
    {
        if (Instance._pools.TryGetValue(go.name, out var pool))
        {
            if (pool.Release(go))
            {
                return true;
            }
        }

        return false;
    }

    public static void ReleaseAll(string name)
    {
        if (Instance._pools.TryGetValue(name, out var pool))
        {
            pool.ReleaseAll();
        }
    }

    public static void ClearPool(string name)
    {
        if (Instance._pools.TryGetValue(name, out var pool))
        {
            pool.Clear();
        }
    }

    public static void RemovePool(string name)
    {
        var instance = Instance;

        if (instance._pools.TryGetValue(name, out var pool))
        {
            pool.Dispose();
            instance._pools.Remove(name);
        }
    }

    public static bool Contains(string name)
    {
        return Instance._pools.ContainsKey(name);
    }

    public static void Clear()
    {
        var instance = Instance;

        foreach (var kvp in instance._pools)
        {
            kvp.Value.Dispose();
        }

        instance._pools.Clear();
    }
}
