using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Pooling;

namespace Vymesy.Projectiles
{
    /// <summary>
    /// Straight-line damage projectile. Returns to the pool when range/lifetime is exceeded
    /// or after first hit (configurable via <see cref="Pierce"/>).
    /// </summary>
    public class Projectile : MonoBehaviour, IPooled
    {
        [SerializeField] private float _hitRadius = 0.25f;
        [SerializeField] private LayerMask _enemyMask = ~0;
        [SerializeField] private int _pierce = 0;
        [SerializeField] private SpriteRenderer _renderer;

        private Vector2 _velocity;
        private float _maxDistance;
        private float _travelled;
        private DamageInfo _damage;
        private int _hitsRemaining;
        private string _poolKey;
        private ProjectilesManager _owner;
        private TrailRenderer _trail;

        public int Pierce { get => _pierce; set => _pierce = value; }

        private void Awake()
        {
            _trail = GetComponent<TrailRenderer>();
        }

        public void Configure(ProjectilesManager owner, string poolKey, Vector2 dir, float speed, float range, in DamageInfo dmg)
        {
            _owner = owner;
            _poolKey = poolKey;
            _velocity = dir.normalized * speed;
            _maxDistance = range;
            _travelled = 0f;
            _damage = dmg;
            _hitsRemaining = 1 + Mathf.Max(0, _pierce);
            transform.right = _velocity.normalized;
            if (_trail != null)
            {
                _trail.Clear();
                _trail.emitting = true;
            }
        }

        public void OnSpawnedFromPool() { }
        public void OnReturnedToPool()
        {
            if (_trail == null) _trail = GetComponent<TrailRenderer>();
            if (_trail == null) return;
            _trail.emitting = false;
            _trail.Clear();
        }

        private void Update()
        {
            float step = _velocity.magnitude * Time.deltaTime;
            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _travelled += step;

            var hits = Physics2D.OverlapCircleAll(transform.position, _hitRadius, _enemyMask);
            for (int i = 0; i < hits.Length; i++)
            {
                var enemyHealth = hits[i].GetComponentInParent<EnemyHealth>();
                if (enemyHealth == null || !enemyHealth.IsAlive) continue;
                enemyHealth.TakeDamage(_damage);
                _hitsRemaining--;
                if (_hitsRemaining <= 0) { Despawn(); return; }
            }

            if (_travelled >= _maxDistance) Despawn();
        }

        private void Despawn()
        {
            if (_owner != null) _owner.Return(_poolKey, gameObject);
            else gameObject.SetActive(false);
        }
    }
}
