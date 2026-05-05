using System.Collections.Generic;
using UnityEngine;

namespace Vymesy.Pooling
{
    public interface IPooled
    {
        void OnSpawnedFromPool();
        void OnReturnedToPool();
    }

    /// <summary>
    /// Simple per-prefab object pool. Inactive objects are kept under the pooler transform.
    /// </summary>
    public class ObjectPooler : MonoBehaviour
    {
        [System.Serializable]
        public class PoolConfig
        {
            public string Key;
            public GameObject Prefab;
            public int Prewarm = 16;
        }

        [SerializeField] private List<PoolConfig> _configs = new List<PoolConfig>();

        private readonly Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        private readonly Dictionary<string, GameObject> _prefabs = new Dictionary<string, GameObject>();

        private void Awake()
        {
            foreach (var c in _configs) Register(c.Key, c.Prefab, c.Prewarm);
        }

        public void Register(string key, GameObject prefab, int prewarm = 0)
        {
            if (string.IsNullOrEmpty(key) || prefab == null) return;
            _prefabs[key] = prefab;
            if (!_pools.TryGetValue(key, out var queue))
            {
                queue = new Queue<GameObject>(Mathf.Max(prewarm, 4));
                _pools[key] = queue;
            }
            for (int i = 0; i < prewarm; i++)
            {
                var go = Instantiate(prefab, transform);
                go.SetActive(false);
                queue.Enqueue(go);
            }
        }

        public GameObject Spawn(string key, Vector3 position, Quaternion rotation)
        {
            if (!_prefabs.TryGetValue(key, out var prefab))
            {
                Debug.LogWarning($"[ObjectPooler] Unknown pool key '{key}'");
                return null;
            }
            if (!_pools.TryGetValue(key, out var queue) || queue.Count == 0)
            {
                queue ??= new Queue<GameObject>();
                _pools[key] = queue;
                var go = Instantiate(prefab, position, rotation, transform);
                NotifySpawned(go);
                return go;
            }
            var instance = queue.Dequeue();
            instance.transform.SetPositionAndRotation(position, rotation);
            instance.SetActive(true);
            NotifySpawned(instance);
            return instance;
        }

        public void Return(string key, GameObject instance)
        {
            if (instance == null) return;
            NotifyReturned(instance);
            instance.SetActive(false);
            instance.transform.SetParent(transform, false);
            if (!_pools.TryGetValue(key, out var queue))
            {
                queue = new Queue<GameObject>();
                _pools[key] = queue;
            }
            queue.Enqueue(instance);
        }

        private static void NotifySpawned(GameObject go)
        {
            var pooled = go.GetComponentsInChildren<IPooled>(true);
            for (int i = 0; i < pooled.Length; i++) pooled[i].OnSpawnedFromPool();
        }

        private static void NotifyReturned(GameObject go)
        {
            var pooled = go.GetComponentsInChildren<IPooled>(true);
            for (int i = 0; i < pooled.Length; i++) pooled[i].OnReturnedToPool();
        }
    }
}
