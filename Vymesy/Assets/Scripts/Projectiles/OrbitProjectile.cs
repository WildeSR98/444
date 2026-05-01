using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Pooling;

namespace Vymesy.Projectiles
{
    public class OrbitProjectile : MonoBehaviour, IPooled
    {
        [SerializeField] private float _hitRadius = 0.3f;
        [SerializeField] private float _hitInterval = 0.4f;
        [SerializeField] private LayerMask _enemyMask = ~0;

        private Transform _center;
        private float _radius;
        private float _angularSpeed;
        private float _angleDeg;
        private float _lifetime;
        private float _age;
        private DamageInfo _damage;
        private string _poolKey;
        private ProjectilesManager _owner;
        private float _nextHitTime;

        public void Configure(ProjectilesManager owner, string poolKey, Transform center, float baseAngleDeg, float radius, float angularSpeed, float lifetime, in DamageInfo dmg)
        {
            _owner = owner;
            _poolKey = poolKey;
            _center = center;
            _radius = radius;
            _angularSpeed = angularSpeed;
            _angleDeg = baseAngleDeg;
            _lifetime = lifetime;
            _age = 0f;
            _damage = dmg;
            _nextHitTime = 0f;
        }

        public void OnSpawnedFromPool() { }
        public void OnReturnedToPool() { }

        private void Update()
        {
            if (_center == null) { Despawn(); return; }
            _angleDeg += _angularSpeed * Time.deltaTime;
            float rad = _angleDeg * Mathf.Deg2Rad;
            transform.position = _center.position + new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f) * _radius;

            _age += Time.deltaTime;
            if (_age >= _lifetime) { Despawn(); return; }

            if (Time.time >= _nextHitTime)
            {
                var hit = Physics2D.OverlapCircle(transform.position, _hitRadius, _enemyMask);
                if (hit != null)
                {
                    var eh = hit.GetComponentInParent<EnemyHealth>();
                    if (eh != null && eh.IsAlive)
                    {
                        eh.TakeDamage(_damage);
                        _nextHitTime = Time.time + _hitInterval;
                    }
                }
            }
        }

        private void Despawn()
        {
            if (_owner != null) _owner.Return(_poolKey, gameObject);
            else gameObject.SetActive(false);
        }
    }
}
