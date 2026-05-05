using UnityEngine;

namespace Vymesy.Enemies.AI
{
    /// <summary>
    /// Brute behaviour: slow advance, then a telegraphed wind-up followed by a heavy charge.
    /// Charging brutes deal extra contact damage on impact.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAIBrute : MonoBehaviour, IEnemyAI
    {
        [SerializeField] private float _windupRange = 4f;
        [SerializeField] private float _windupDuration = 0.8f;
        [SerializeField] private float _chargeDuration = 0.5f;
        [SerializeField] private float _chargeSpeedMultiplier = 4f;
        [SerializeField] private float _chargeDamageMultiplier = 1.6f;

        private Rigidbody2D _rb;
        private EnemyDefinition _def;
        private Transform _target;
        private float _difficultyMultiplier = 1f;
        private float _stateUntil;
        private bool _windingUp;
        private bool _charging;
        private Vector2 _chargeDir;

        public void Initialize(EnemyDefinition def, Transform target, float difficultyMultiplier)
        {
            _def = def;
            _target = target;
            _difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier);
            _windingUp = _charging = false;
            _stateUntil = 0f;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null) { _rb.gravityScale = 0f; _rb.freezeRotation = true; }
        }

        private void Update()
        {
            if (_def == null || _target == null) return;
            float distance = Vector2.Distance(transform.position, _target.position);
            float now = Time.time;

            if (_charging)
            {
                if (now >= _stateUntil) { _charging = false; }
                return;
            }

            if (_windingUp)
            {
                if (now >= _stateUntil)
                {
                    _windingUp = false;
                    _charging = true;
                    _chargeDir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
                    if (_chargeDir.sqrMagnitude < 0.001f) _chargeDir = Vector2.right;
                    _stateUntil = now + _chargeDuration;
                }
                return;
            }

            if (distance <= _windupRange)
            {
                _windingUp = true;
                _stateUntil = now + _windupDuration;
            }
        }

        private void FixedUpdate()
        {
            if (_def == null || _target == null) return;
            Vector2 velocity;
            if (_windingUp) velocity = Vector2.zero;
            else if (_charging) velocity = _chargeDir * _def.MoveSpeed * _chargeSpeedMultiplier;
            else
            {
                Vector2 dir = ((Vector2)_target.position - (Vector2)transform.position).normalized;
                velocity = dir * _def.MoveSpeed;
            }
            SetVelocity(velocity);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_def == null) return;
            var ph = collision.collider.GetComponent<Vymesy.Player.PlayerHealth>();
            if (ph == null) return;
            float dmg = _def.ContactDamage * _difficultyMultiplier * (_charging ? _chargeDamageMultiplier : 1f);
            ph.TakeDamage(Vymesy.Damage.DamageInfo.Physical(dmg, gameObject));
            if (_charging) _charging = false; // single big hit per charge
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
