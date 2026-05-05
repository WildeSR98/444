using UnityEngine;

namespace Vymesy.Enemies.AI
{
    /// <summary>
    /// Stalker behaviour: orbits at a comfortable distance, then occasionally dashes through
    /// the target. Replaces the default <see cref="EnemyAI"/> on a prefab.
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class EnemyAIStalker : MonoBehaviour, IEnemyAI
    {
        [SerializeField] private float _orbitDistance = 4.5f;
        [SerializeField] private float _orbitSpeedMultiplier = 0.9f;
        [SerializeField] private float _dashSpeedMultiplier = 3.0f;
        [SerializeField] private float _dashChancePerSecond = 0.18f;
        [SerializeField] private float _dashDuration = 0.5f;

        private Rigidbody2D _rb;
        private EnemyDefinition _def;
        private Transform _target;
        private float _difficultyMultiplier = 1f;
        private float _dashUntil;
        private int _orbitDirection = 1;

        public void Initialize(EnemyDefinition def, Transform target, float difficultyMultiplier)
        {
            _def = def;
            _target = target;
            _difficultyMultiplier = Mathf.Max(0.1f, difficultyMultiplier);
            _orbitDirection = Random.value < 0.5f ? -1 : 1;
        }

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            if (_rb != null) { _rb.gravityScale = 0f; _rb.freezeRotation = true; }
        }

        private void Update()
        {
            if (_def == null || _target == null) return;
            if (Time.time < _dashUntil) return;
            // Roll for dash each frame, scaled to chance-per-second.
            if (Random.value < _dashChancePerSecond * Time.deltaTime)
            {
                _dashUntil = Time.time + _dashDuration;
            }
        }

        private void FixedUpdate()
        {
            if (_def == null || _target == null) return;
            Vector2 toTarget = (Vector2)(_target.position - transform.position);
            float distance = toTarget.magnitude;
            if (distance < 0.001f) return;
            Vector2 dir = toTarget / distance;

            Vector2 velocity;
            if (Time.time < _dashUntil)
            {
                velocity = dir * _def.MoveSpeed * _dashSpeedMultiplier;
            }
            else
            {
                Vector2 tangent = new Vector2(-dir.y, dir.x) * _orbitDirection;
                float radial = (distance - _orbitDistance) * 1.5f;
                velocity = (tangent + dir * radial).normalized * _def.MoveSpeed * _orbitSpeedMultiplier;
            }
            SetVelocity(velocity);
        }

        private void OnCollisionStay2D(Collision2D collision)
        {
            if (_def == null) return;
            var ph = collision.collider.GetComponent<Vymesy.Player.PlayerHealth>();
            if (ph == null) return;
            ph.TakeDamage(Vymesy.Damage.DamageInfo.Physical(_def.ContactDamage * _difficultyMultiplier, gameObject));
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
