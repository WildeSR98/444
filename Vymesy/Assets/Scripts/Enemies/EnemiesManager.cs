using System.Collections.Generic;
using UnityEngine;
using Vymesy.Pooling;
using Vymesy.Utils;

namespace Vymesy.Enemies
{
    /// <summary>
    /// Spawns waves of вымесы around the player and tracks alive enemies.
    /// </summary>
    public class EnemiesManager : MonoBehaviour
    {
        [System.Serializable]
        public class EnemyEntry
        {
            public EnemyDefinition Definition;
            public GameObject Prefab;
            public int SpawnWeight = 10;
            [Tooltip("Earliest wave at which this entry is eligible.")]
            public int MinWave = 0;
        }

        [Header("Spawn Settings")]
        [SerializeField] private List<EnemyEntry> _entries = new List<EnemyEntry>();
        [SerializeField] private Transform _target;
        [SerializeField] private float _baseSpawnInterval = 1.2f;
        [SerializeField] private float _minSpawnInterval = 0.15f;
        [SerializeField] private int _maxAlive = 200;
        [SerializeField] private float _spawnRingMin = 8f;
        [SerializeField] private float _spawnRingMax = 12f;
        [SerializeField] private ObjectPooler _pooler;

        public IReadOnlyDictionary<int, EnemyController> AliveEnemies => _alive;
        public float AscensionMultiplier { get; private set; } = 1f;

        public void SetAscensionMultiplier(float m) => AscensionMultiplier = Mathf.Max(0.1f, m);

        private readonly Dictionary<int, EnemyController> _alive = new Dictionary<int, EnemyController>();
        // Deferred-removal queue: filled by OnEnemyDied during AoE iteration, drained at end of Update.
        // Prevents InvalidOperationException when AoE damage kills enemies mid-foreach.
        private readonly List<PendingRemoval> _pendingRemovals = new List<PendingRemoval>();
        private float _spawnTimer;
        private bool _spawning;
        private int _difficultyWave;

        private struct PendingRemoval
        {
            public int InstanceId;
            public string PoolKey;
            public GameObject GameObject;
        }

        private void Awake()
        {
            if (_pooler == null) _pooler = GetComponentInChildren<ObjectPooler>();
            if (_pooler == null)
            {
                var pgo = new GameObject("EnemyPool");
                pgo.transform.SetParent(transform, false);
                _pooler = pgo.AddComponent<ObjectPooler>();
            }
            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (e == null || e.Prefab == null || e.Definition == null) continue;
                _pooler.Register(PoolKeyFor(e), e.Prefab, prewarm: 8);
            }
        }

        public void SetTarget(Transform target) => _target = target;

        /// <summary>
        /// Runtime-friendly registration of an enemy entry. Used by VymesyDemoBootstrap and tools
        /// that build the enemy roster procedurally instead of via the inspector.
        /// </summary>
        public void AddEntry(EnemyDefinition def, GameObject prefab, int spawnWeight = 10, int minWave = 0, int prewarm = 8)
        {
            if (def == null || prefab == null) return;
            var entry = new EnemyEntry { Definition = def, Prefab = prefab, SpawnWeight = spawnWeight, MinWave = minWave };
            _entries.Add(entry);
            if (_pooler == null) _pooler = GetComponentInChildren<ObjectPooler>();
            if (_pooler != null) _pooler.Register(PoolKeyFor(entry), prefab, prewarm);
        }

        public void ConfigureSpawn(float baseInterval, float minInterval, float ringMin, float ringMax, int maxAlive)
        {
            _baseSpawnInterval = baseInterval;
            _minSpawnInterval = minInterval;
            _spawnRingMin = ringMin;
            _spawnRingMax = ringMax;
            _maxAlive = maxAlive;
        }

        public void BeginSpawning()
        {
            _spawning = true;
            _spawnTimer = 0f;
            _difficultyWave = 0;
        }

        public void StopSpawning() => _spawning = false;

        public void SetDifficulty(int wave) => _difficultyWave = Mathf.Max(0, wave);

        public void ClearAll()
        {
            foreach (var kv in _alive)
            {
                if (kv.Value == null) continue;
                _pooler.Return(KeyForController(kv.Value), kv.Value.gameObject);
            }
            _alive.Clear();
            // Drop any deferred removals — their GameObjects were already returned above (or
            // are being torn down with the run), so a later DrainPendingRemovals() call would
            // double-return them to the pool, corrupting reuse on the next run.
            _pendingRemovals.Clear();
        }

        private void Update()
        {
            DrainPendingRemovals();
            if (!_spawning || _target == null) return;
            if (_alive.Count >= _maxAlive) return;

            float interval = Mathf.Max(_minSpawnInterval, _baseSpawnInterval * Mathf.Pow(0.92f, _difficultyWave));
            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= interval)
            {
                _spawnTimer = 0f;
                SmartSpawn();
            }
        }

        private void LateUpdate()
        {
            // Late-update drain catches deaths that occurred during other systems' updates
            // (towers, skill triggers) so the dictionary is consistent before the next frame.
            DrainPendingRemovals();
        }

        private void DrainPendingRemovals()
        {
            if (_pendingRemovals.Count == 0) return;
            for (int i = 0; i < _pendingRemovals.Count; i++)
            {
                var r = _pendingRemovals[i];
                _alive.Remove(r.InstanceId);
                if (r.GameObject != null && _pooler != null) _pooler.Return(r.PoolKey, r.GameObject);
            }
            _pendingRemovals.Clear();
        }

        private void SmartSpawn()
        {
            var entry = PickEntry();
            if (entry == null) return;

            Vector2 offset = MathUtils.RandomInAnnulus(_spawnRingMin, _spawnRingMax);
            Vector3 position = _target.position + (Vector3)offset;

            string key = PoolKeyFor(entry);
            var go = _pooler.Spawn(key, position, Quaternion.identity);
            if (go == null) return;
            var ctrl = go.GetComponent<EnemyController>();
            if (ctrl == null)
            {
                Debug.LogWarning($"[EnemiesManager] Spawned prefab {go.name} has no EnemyController");
                _pooler.Return(key, go);
                return;
            }
            float multiplier = (1f + _difficultyWave * 0.15f) * AscensionMultiplier;
            ctrl.Configure(entry.Definition, this, key, _target, multiplier);
            _alive[ctrl.GetInstanceID()] = ctrl;
        }

        /// <summary>
        /// Force-spawn the first registered entry whose definition matches the given type.
        /// Returns the spawned controller (or null if none registered / pool is full).
        /// </summary>
        public EnemyController SpawnSpecific(EnemyType type, Vector3? worldPos = null)
        {
            EnemyEntry entry = null;
            for (int i = 0; i < _entries.Count; i++)
            {
                var e = _entries[i];
                if (e == null || e.Definition == null) continue;
                if (e.Definition.Type == type) { entry = e; break; }
            }
            if (entry == null || _target == null || _pooler == null) return null;

            Vector3 position = worldPos ?? (_target.position + (Vector3)MathUtils.RandomInAnnulus(_spawnRingMin, _spawnRingMax));
            string key = PoolKeyFor(entry);
            var go = _pooler.Spawn(key, position, Quaternion.identity);
            if (go == null) return null;
            var ctrl = go.GetComponent<EnemyController>();
            if (ctrl == null) { _pooler.Return(key, go); return null; }
            float multiplier = (1f + _difficultyWave * 0.15f) * AscensionMultiplier;
            ctrl.Configure(entry.Definition, this, key, _target, multiplier);
            _alive[ctrl.GetInstanceID()] = ctrl;
            return ctrl;
        }

        public void OnEnemyDied(EnemyController ctrl, string poolKey)
        {
            if (ctrl == null) return;
            // Defer the dictionary mutation until the next Update tick. Synchronous removal
            // here would corrupt any foreach over AliveEnemies that triggered the death
            // (e.g. AoETower, AuraTower, AoESkill, NovaSkill, PoisonTower).
            _pendingRemovals.Add(new PendingRemoval
            {
                InstanceId = ctrl.GetInstanceID(),
                PoolKey = poolKey,
                GameObject = ctrl.gameObject,
            });
        }

        private EnemyEntry PickEntry()
        {
            int n = _entries.Count;
            if (n == 0) return null;
            int[] weights = new int[n];
            for (int i = 0; i < n; i++)
            {
                var e = _entries[i];
                if (e == null || e.Prefab == null || e.Definition == null) { weights[i] = 0; continue; }
                if (_difficultyWave < e.MinWave) { weights[i] = 0; continue; }
                weights[i] = Mathf.Max(0, e.SpawnWeight);
            }
            int idx = MathUtils.WeightedPick(weights);
            return idx >= 0 ? _entries[idx] : null;
        }

        private static string PoolKeyFor(EnemyEntry e)
        {
            return $"enemy::{e.Definition.Type}::{e.Definition.name}";
        }

        private static string KeyForController(EnemyController ctrl)
        {
            return $"enemy::{ctrl.Definition.Type}::{ctrl.Definition.name}";
        }
    }
}
