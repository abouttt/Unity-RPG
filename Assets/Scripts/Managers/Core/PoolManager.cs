using System;
using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public sealed class PoolManager : MonoBehaviourSingleton<PoolManager>
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
        private readonly Stack<GameObject> _inactiveObjects = new();

        public Pool(GameObject prefab, int count, string tag)
        {
            _prefab = prefab;
            _root = new GameObject($"{tag}_Root").transform;

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

            var go = _inactiveObjects.Pop();
            go.transform.SetParent(parent != null ? parent : _root);
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

            PushToInactiveContainer(go);

            return true;
        }

        public void ReleaseAll()
        {
            foreach (var go in _activeObjects)
            {
                PushToInactiveContainer(go);
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
            _prefab = null;
            Destroy(_root.gameObject);
        }

        private void Create()
        {
            var go = Instantiate(_prefab);
            go.name = _prefab.name;
            PushToInactiveContainer(go);
            _count++;
        }

        private void PushToInactiveContainer(GameObject go)
        {
            go.SetActive(false);
            go.transform.SetParent(_root);
            _inactiveObjects.Push(go);
        }
    }
    #endregion

    [SerializeField, ReadOnly, SerializedDictionary("Tag", "Pool")]
    private SerializedDictionary<string, Pool> _pools = new();

    protected override void Dispose()
    {
        base.Dispose();
        Clear();
    }

    public static void CreatePool(GameObject prefab, int count = 3, string tag = null)
    {
        var instance = Instance;

        if (string.IsNullOrEmpty(tag))
        {
            tag = prefab.name;
        }

        if (instance._pools.ContainsKey(tag))
        {
            Debug.LogWarning($"[PoolManager.CreatePool] {tag} pool already exist.");
            return;
        }

        var pool = new Pool(prefab, count, tag);
        pool.Root.SetParent(instance.transform);
        instance._pools.Add(tag, pool);
    }

    public static GameObject Get(GameObject go, Transform parent = null)
    {
        if (go == null)
        {
            Debug.LogWarning($"[PoolManager.Get] GameObject is null.");
            return null;
        }

        var instance = Instance;

        if (!instance._pools.TryGetValue(go.name, out var pool))
        {
            CreatePool(go);
            pool = instance._pools[go.name];
        }

        return pool.Get(parent);
    }

    public static GameObject Get(string tag, Transform parent = null)
    {
        if (Instance._pools.TryGetValue(tag, out var pool))
        {
            return pool.Get(parent);
        }
        else
        {
            return null;
        }
    }

    public static void Release(GameObject go, string tag = null)
    {
        if (!go.activeSelf)
        {
            return;
        }

        if (string.IsNullOrEmpty(tag))
        {
            tag = go.name;
        }

        if (Instance._pools.TryGetValue(tag, out var pool))
        {
            if (pool.Release(go))
            {
                return;
            }
        }

        Destroy(go);
    }

    public static void ReleaseAll(string tag)
    {
        if (Instance._pools.TryGetValue(tag, out var pool))
        {
            pool.ReleaseAll();
        }
    }

    public static void ClearPool(string tag)
    {
        if (Instance._pools.TryGetValue(tag, out var pool))
        {
            pool.Clear();
        }
    }

    public static void RemovePool(string tag)
    {
        var instance = Instance;

        if (instance._pools.TryGetValue(tag, out var pool))
        {
            pool.Dispose();
            instance._pools.Remove(tag);
        }
    }

    public static bool Contains(string tag)
    {
        return Instance._pools.ContainsKey(tag);
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
