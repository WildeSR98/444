using UnityEngine;
using Vymesy.Damage;
using Vymesy.Projectiles;

namespace Vymesy.Enemies.AI
{
    /// <summary>
    /// Boss behaviour: slow pursuit, periodic radial bullet pattern, periodic minion summon.
    /// Phase changes when health drops below 50% — fires faster and summons more.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(EnemyHealth))]
    public class EnemyAIBoss : MonoBehaviour, IEnemyAI
    {
        [SerializeField] private float _bulletPatternInterval = 4.5f;
        [SerializeField] private int _bulletsPerPattern = 12;
        [SerializeField] private float _bulletSpeed = 4f;
        [SerializeField] private float _minionInterval = 8f;
        [SerializeField] private string _projectilePoolKey = "proj_enemy";

        private Rigidbody2D _rb;
        private EnemyDefinition _def;
        private EnemyHealth _health;
        private Transform _target;
        private float _difficultyMultiplier = 1f;
        private float _nextBulletTime;
        private float _nextMinionTime;
        private bool _phase2;

        public void Initialize(EnemyDefinition def, Transform target, float difficultyMultiplier)
        {
            _def = def;
            _target = target;
            _difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier);
            _nextBulletTime = Time.time + 2f;
            _nextMinionTime = Time.time + 4f;
            _phase2 = false;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _health = GetComponent<EnemyHealth>();
            if (_rb != null) { _rb.gravityScale = 0f; _rb.freezeRotation = true; }
        }

        private void Update()
        {
            if (_def == null || _target == null) return;

            if (!_phase2 && _health != null && _health.MaxHealth > 0f && _health.CurrentHealth / _health.MaxHealth < 0.5f)
            {
                _phase2 = true;
                _bulletPatternInterval *= 0.6f;
                _bulletsPerPattern += 6;
                _minionInterval *= 0.7f;
            }

            if (Time.time >= _nextBulletTime)
            {
                _nextBulletTime = Time.time + _bulletPatternInterval;
                FireRadialPattern();
            }
            if (Time.time >= _nextMinionTime)
            {
                _nextMinionTime = Time.time + _minionInterval;
                SummonMinions();
            }
        }

        private void FixedUpdate()
        {
            if (_def == null || _target == null) return;
            Vector2 toTarget = (Vector2)_target.position - (Vector2)transform.position;
            float distance = toTarget.magnitude;
            if (distance < 0.001f) return;
            Vector2 dir = toTarget / distance;
            float speed = _def.MoveSpeed * (distance > 6f ? 1f : 0.4f);
            SetVelocity(dir * speed);
        }

        private void FireRadialPattern()
        {
            if (!ProjectilesManager.HasInstance) return;
            var pm = ProjectilesManager.Instance;
            int n = _bulletsPerPattern;
            float dmg = _def.ContactDamage * _difficultyMultiplier * 0.5f;
            var info = new DamageInfo { Amount = dmg, Type = DamageType.Physical, Source = gameObject };
            for (int i = 0; i < n; i++)
            {
                float ang = (i / (float)n) * Mathf.PI * 2f;
                Vector2 dir = new Vector2(Mathf.Cos(ang), Mathf.Sin(ang));
                pm.Fire(_projectilePoolKey, transform.position, dir, _bulletSpeed, 8f, info);
            }
        }

        private void SummonMinions()
        {
            // Fire-and-forget: ask the singleton EnemiesManager to schedule a small wave at our position.
            var em = UnityEngine.Object.FindObjectOfType<EnemiesManager>();
            if (em == null) return;
            // Increment difficulty wave temporarily — cheap way to pull more enemies.
            em.SetDifficulty(em != null ? Mathf.Max(2, em.AliveEnemies.Count > 0 ? em.AliveEnemies.Count / 8 : 2) : 2);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_def == null) return;
            var ph = collision.collider.GetComponent<Vymesy.Player.PlayerHealth>();
            if (ph == null) return;
            ph.TakeDamage(DamageInfo.Physical(_def.ContactDamage * _difficultyMultiplier, gameObject));
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
}
