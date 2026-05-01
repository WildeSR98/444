using System.Collections.Generic;
using UnityEngine;
using Vymesy.Damage;
using Vymesy.Pooling;
using Vymesy.Utils;

namespace Vymesy.Projectiles
{
    public class ProjectilesManager : Singleton<ProjectilesManager>
    {
        [System.Serializable]
        public class PoolEntry
        {
            public string Key;
            public GameObject Prefab;
            public int Prewarm = 32;
        }

        [SerializeField] private List<PoolEntry> _entries = new List<PoolEntry>();
        [SerializeField] private ObjectPooler _pooler;

        protected override void OnAwake()
        {
            if (_pooler == null) _pooler = GetComponentInChildren<ObjectPooler>();
            if (_pooler == null)
            {
                var go = new GameObject("ProjectilePool");
                go.transform.SetParent(transform, false);
                _pooler = go.AddComponent<ObjectPooler>();
            }
            foreach (var e in _entries)
            {
                if (e == null || string.IsNullOrEmpty(e.Key) || e.Prefab == null) continue;
                _pooler.Register(e.Key, e.Prefab, e.Prewarm);
            }
        }

        public void Register(string key, GameObject prefab, int prewarm = 16) => _pooler.Register(key, prefab, prewarm);

        public void Fire(string key, Vector2 origin, Vector2 dir, float speed, float range, in DamageInfo dmg)
        {
            var go = _pooler.Spawn(key, origin, Quaternion.identity);
            if (go == null) return;
            var p = go.GetComponent<Projectile>();
            if (p != null) { p.Configure(this, key, dir, speed, range, dmg); return; }
            var hp = go.GetComponent<HomingProjectile>();
            if (hp != null) { hp.Configure(this, key, dir, speed, range, dmg); return; }
            Debug.LogWarning($"[ProjectilesManager] Prefab for '{key}' has no Projectile component.");
            _pooler.Return(key, go);
        }

        public void FireOrbit(string key, Transform center, float baseAngleDeg, float radius, float angularSpeed, float lifetime, in DamageInfo dmg)
        {
            var go = _pooler.Spawn(key, center.position, Quaternion.identity);
            if (go == null) return;
            var orb = go.GetComponent<OrbitProjectile>();
            if (orb == null)
            {
                Debug.LogWarning($"[ProjectilesManager] Prefab for '{key}' has no OrbitProjectile.");
                _pooler.Return(key, go);
                return;
            }
            orb.Configure(this, key, center, baseAngleDeg, radius, angularSpeed, lifetime, dmg);
        }

        public void Return(string key, GameObject instance) => _pooler.Return(key, instance);
    }
}
