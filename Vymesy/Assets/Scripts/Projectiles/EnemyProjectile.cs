using UnityEngine;
using Vymesy.Damage;
using Vymesy.Player;
using Vymesy.Pooling;

namespace Vymesy.Projectiles
{
    /// <summary>
    /// Hostile-to-player projectile fired by enemies (e.g. boss radial pattern). Mirrors
    /// <see cref="Projectile"/>'s straight-line behaviour but checks for <see cref="PlayerHealth"/>
    /// rather than <see cref="Vymesy.Enemies.EnemyHealth"/> so it actually damages the player.
    /// </summary>
    public class EnemyProjectile : MonoBehaviour, IPooled
    {
        [SerializeField] private float _hitRadius = 0.25f;
        [SerializeField] private LayerMask _playerMask = ~0;
        [SerializeField] private SpriteRenderer _renderer;

        private Vector2 _velocity;
        private float _maxDistance;
        private float _travelled;
        private DamageInfo _damage;
        private string _poolKey;
        private ProjectilesManager _owner;

        public void Configure(ProjectilesManager owner, string poolKey, Vector2 dir, float speed, float range, in DamageInfo dmg)
        {
            _owner = owner;
            _poolKey = poolKey;
            _velocity = dir.normalized * speed;
            _maxDistance = range;
            _travelled = 0f;
            _damage = dmg;
            transform.right = _velocity.normalized;
        }

        public void OnSpawnedFromPool() { }
        public void OnReturnedToPool() { }

        private void Update()
        {
            float step = _velocity.magnitude * Time.deltaTime;
            transform.position += (Vector3)(_velocity * Time.deltaTime);
            _travelled += step;

            var hits = Physics2D.OverlapCircleAll(transform.position, _hitRadius, _playerMask);
            for (int i = 0; i < hits.Length; i++)
            {
                var ph = hits[i].GetComponentInParent<PlayerHealth>();
                if (ph == null || !ph.IsAlive) continue;
                ph.TakeDamage(_damage);
                Despawn();
                return;
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
