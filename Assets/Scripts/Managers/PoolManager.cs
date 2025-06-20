using System.Collections.Generic;
using UnityEngine;
using AYellowpaper.SerializedCollections;

public class PoolManager : MonoSingleton<PoolManager>, IManager
{
    #region Pool
    private class Pool
    {
        public Transform Root => _root;

        private readonly GameObject _prefab;
        private readonly Transform _root;
        private readonly HashSet<GameObject> _activeObjects = new();
        private readonly Stack<GameObject> _inactiveObjects = new();

        public Pool(GameObject prefab, int count)
        {
            _prefab = prefab;
            _root = new GameObject($"{prefab}_Root").transform;

            for (int i = 0; i < count; i++)
            {
                ReturnToInactive(Create());
            }
        }

        public GameObject Get(Transform parent)
        {
            GameObject gameObject;

            if (_inactiveObjects.Count > 0)
            {
                gameObject = _inactiveObjects.Pop();
                if (gameObject == null)
                {
                    gameObject = Create();
                }
            }
            else
            {
                gameObject = Create();
            }

            gameObject.transform.SetParent(parent == null ? _root : parent);
            gameObject.SetActive(true);
            _activeObjects.Add(gameObject);

            return gameObject;
        }

        public bool Return(GameObject gameObject)
        {
            if (!_activeObjects.Remove(gameObject))
            {
                return false;
            }

            ReturnToInactive(gameObject);

            return true;
        }

        public void ReturnAll()
        {
            foreach (var obj in _activeObjects)
            {
                ReturnToInactive(obj);
            }

            _activeObjects.Clear();
        }

        public void Clear()
        {
            foreach (var obj in _activeObjects)
            {
                Destroy(obj);
            }

            foreach (var obj in _inactiveObjects)
            {
                Destroy(obj);
            }

            _activeObjects.Clear();
            _inactiveObjects.Clear();
        }

        public void Dispose()
        {
            Clear();
            Destroy(_root.gameObject);
        }

        private GameObject Create()
        {
            var newGameObject = Instantiate(_prefab);
            newGameObject.name = _prefab.name;
            return newGameObject;
        }

        private void ReturnToInactive(GameObject gameObject)
        {
            gameObject.SetActive(false);
            gameObject.transform.SetParent(_root);
            _inactiveObjects.Push(gameObject);
        }
    }
    #endregion

    [SerializeField, ReadOnly]
    private SerializedDictionary<string, Pool> _pools = new();

    public void Initialize()
    {
    }

    public void CreatePool(GameObject prefab, int count = 5)
    {
        if (prefab == null)
        {
            return;
        }

        var name = prefab.name;

        if (_pools.ContainsKey(name))
        {
            Debug.LogWarning($"[PoolManager] {name} Pool already exist.");
            return;
        }

        var newPool = new Pool(prefab, count);
        newPool.Root.SetParent(transform);
        _pools.Add(name, newPool);
    }

    public GameObject Get(GameObject gameObject, Transform parent = null)
    {
        if (gameObject == null)
        {
            return null;
        }

        if (!_pools.TryGetValue(gameObject.name, out var pool))
        {
            CreatePool(gameObject);
            pool = _pools[gameObject.name];
        }

        return pool.Get(parent);
    }

    public GameObject Get(string name, Transform parent = null)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            return pool.Get(parent);
        }

        return null;
    }

    public bool Return(GameObject gameObject)
    {
        if (gameObject == null)
        {
            return false;
        }

        if (_pools.TryGetValue(gameObject.name, out var pool))
        {
            if (pool.Return(gameObject))
            {
                return true;
            }
        }

        return false;
    }

    public void ReturnAll(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.ReturnAll();
        }
    }

    public void ClearPool(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Clear();
        }
    }

    public void RemovePool(string name)
    {
        if (_pools.TryGetValue(name, out var pool))
        {
            pool.Dispose();
            _pools.Remove(name);
        }
    }

    public bool Contains(string name)
    {
        return _pools.ContainsKey(name);
    }

    public void Clear()
    {
        foreach (var pool in _pools.Values)
        {
            pool.Dispose();
        }

        _pools.Clear();
    }
}
