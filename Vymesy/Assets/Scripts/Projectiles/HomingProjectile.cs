using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Pooling;

namespace Vymesy.Projectiles
{
    /// <summary>
    /// Variant projectile that bends toward the nearest enemy each frame.
    /// </summary>
    public class HomingProjectile : MonoBehaviour, IPooled
    {
        [SerializeField] private float _hitRadius = 0.25f;
        [SerializeField] private float _turnRateDegPerSec = 240f;
        [SerializeField] private float _scanRadius = 8f;
        [SerializeField] private LayerMask _enemyMask = ~0;

        private Vector2 _velocity;
        private float _speed;
        private float _maxDistance;
        private float _travelled;
        private DamageInfo _damage;
        private string _poolKey;
        private ProjectilesManager _owner;
        private Transform _target;

        public void Configure(ProjectilesManager owner, string poolKey, Vector2 dir, float speed, float range, in DamageInfo dmg)
        {
            _owner = owner;
            _poolKey = poolKey;
            _speed = speed;
            _velocity = dir.normalized * speed;
            _maxDistance = range;
            _travelled = 0f;
            _damage = dmg;
            _target = null;
            transform.right = _velocity.normalized;
        }

        public void OnSpawnedFromPool() { }
        public void OnReturnedToPool() { _target = null; }

        private void Update()
        {
            if (_target == null || !_target.gameObject.activeInHierarchy) AcquireTarget();

            if (_target != null)
            {
                Vector2 desired = ((Vector2)_target.position - (Vector2)transform.position).normalized * _speed;
                float maxDelta = _turnRateDegPerSec * Mathf.Deg2Rad * Time.deltaTime;
                _velocity = Vector2.MoveTowards(_velocity, desired, _speed * maxDelta);
                if (_velocity.sqrMagnitude > 0.001f) transform.right = _velocity.normalized;
            }

            float step = _velocity.magnitude * Time.deltaTime;
            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _travelled += step;

            var hit = Physics2D.OverlapCircle(transform.position, _hitRadius, _enemyMask);
            if (hit != null)
            {
                var eh = hit.GetComponentInParent<EnemyHealth>();
                if (eh != null && eh.IsAlive)
                {
                    eh.TakeDamage(_damage);
                    Despawn();
                    return;
                }
            }
            if (_travelled >= _maxDistance) Despawn();
        }

        private void AcquireTarget()
        {
            var hits = Physics2D.OverlapCircleAll(transform.position, _scanRadius, _enemyMask);
            float bestSqr = float.MaxValue;
            Transform best = null;
            for (int i = 0; i < hits.Length; i++)
            {
                var h = hits[i];
                if (h == null) continue;
                var eh = h.GetComponentInParent<EnemyHealth>();
                if (eh == null || !eh.IsAlive) continue;
                float sqr = ((Vector2)h.transform.position - (Vector2)transform.position).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; best = h.transform; }
            }
            _target = best;
        }

        private void Despawn()
        {
            if (_owner != null) _owner.Return(_poolKey, gameObject);
            else gameObject.SetActive(false);
        }
    }
}
