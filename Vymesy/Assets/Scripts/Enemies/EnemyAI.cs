using UnityEngine;
using Vymesy.Damage;
using Vymesy.Player;

namespace Vymesy.Enemies
{
    /// <summary>
    /// Simple homing AI: walks toward target, contacts deal damage on cooldown,
    /// optional ranged variant fires projectiles when within range.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAI : MonoBehaviour, IEnemyAI
    {
        [SerializeField] private Rigidbody2D _rb;

        private EnemyDefinition _def;
        private Transform _target;
        private float _nextAttackTime;
        private float _difficultyMultiplier = 1f;

        public void Initialize(EnemyDefinition def, Transform target, float difficultyMultiplier)
        {
            _def = def;
            _target = target;
            _difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier);
            _nextAttackTime = 0f;
        }

        private void Reset() => _rb = GetComponent<Rigidbody2D>();

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                _rb.gravityScale = 0f;
                _rb.freezeRotation = true;
            }
        }

        private void FixedUpdate()
        {
            if (_def == null || _target == null) return;
            Vector2 toTarget = (Vector2)(_target.position - transform.position);
            float distance = toTarget.magnitude;
            if (distance < 0.001f) return;

            Vector2 dir = toTarget / distance;
            float speed = _def.MoveSpeed;

            if (_def.IsRanged && distance < _def.AttackRange * 0.9f)
            {
                speed *= 0.2f;
            }

            SetVelocity(dir * speed);
        }

        private void Update()
        {
            if (_def == null || _target == null) return;

            if (Time.time < _nextAttackTime) return;

            if (_def.IsRanged)
            {
                float dist = Vector2.Distance(transform.position, _target.position);
                if (dist <= _def.AttackRange) FireRangedAttack();
            }
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_def == null || _def.IsRanged) return;
            if (Time.time < _nextAttackTime) return;
            var dmg = collision.collider.GetComponent<PlayerHealth>();
            if (dmg == null) return;
            dmg.TakeDamage(DamageInfo.Physical(_def.ContactDamage * _difficultyMultiplier, gameObject));
            _nextAttackTime = Time.time + _def.AttackInterval;
        }

        private void FireRangedAttack()
        {
            // Hook for projectile system; left as no-op when no projectile pool key configured.
            _nextAttackTime = Time.time + _def.AttackInterval;
            if (string.IsNullOrEmpty(_def.ProjectilePoolKey)) return;
            // Projectile firing is delegated to a component the project can plug in
            // (so EnemyAI does not hard-depend on a specific projectile implementation).
            SendMessage("FireEnemyProjectile", new EnemyProjectileRequest
            {
                Direction = ((Vector2)(_target.position - transform.position)).normalized,
                Damage = _def.ContactDamage * _difficultyMultiplier,
                Speed = _def.ProjectileSpeed,
                PoolKey = _def.ProjectilePoolKey,
            }, SendMessageOptions.DontRequireReceiver);
        }

        private void SetVelocity(Vector2 v)
        {
            if (_rb == null) return;
#if UNITY_6000_0_OR_NEWER
            _rb.linearVelocity = v;
#else
            _rb.velocity = v;
#endif
        }
    }

    public struct EnemyProjectileRequest
    {
        public Vector2 Direction;
        public float Damage;
        public float Speed;
        public string PoolKey;
    }
}
