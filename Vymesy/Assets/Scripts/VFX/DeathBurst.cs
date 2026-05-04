using UnityEngine;
using Vymesy.Demo;
using Vymesy.Enemies;

namespace Vymesy.VFX
{
    /// <summary>
    /// Spawns short-lived particle sprites flying outward when the host enemy dies.
    /// Uses pure code so it works without any imported textures/materials.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class DeathBurst : MonoBehaviour
    {
        [SerializeField] private int _particles = 14;
        [SerializeField] private float _speed = 5.5f;
        [SerializeField] private float _lifetime = 0.62f;
        [SerializeField] private float _scale = 0.16f;

        private EnemyHealth _health;
        private SpriteRenderer _renderer;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _renderer = GetComponentInChildren<SpriteRenderer>();
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnDied += HandleDied;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDied -= HandleDied;
        }

        private void HandleDied()
        {
            Color color = _renderer != null ? _renderer.color : Color.white;
            Vector3 origin = transform.position;
            for (int i = 0; i < _particles; i++)
            {
                float angle = (i / (float)_particles) * Mathf.PI * 2f + Random.value * 0.3f;
                Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));
                Particle.Spawn(origin, dir * _speed * Random.Range(0.7f, 1.2f), _lifetime, _scale, color, i % 2 == 0);
            }
        }

        // Inner runtime particle to avoid pulling in Unity's ParticleSystem (which needs material assets).
        private class Particle : MonoBehaviour
        {
            private Vector2 _velocity;
            private float _life;
            private float _maxLife;
            private SpriteRenderer _sr;
            private Color _baseColor;

            public static void Spawn(Vector3 origin, Vector2 velocity, float life, float scale, Color color, bool spark)
            {
                var go = new GameObject("DeathParticle");
                go.transform.position = origin;
                go.transform.localScale = Vector3.one * scale;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = DemoSprites.Get(spark ? DemoSprites.Shape.Diamond : DemoSprites.Shape.Circle, color, 8);
                sr.sortingOrder = 6;
                var p = go.AddComponent<Particle>();
                p._sr = sr;
                p._baseColor = color;
                p._velocity = velocity;
                p._maxLife = life;
                p._life = life;
            }

            private void Update()
            {
                _life -= Time.deltaTime;
                if (_life <= 0f) { Destroy(gameObject); return; }
                transform.position += (Vector3)(_velocity * Time.deltaTime);
                _velocity *= 0.92f;
                if (_sr != null)
                {
                    var c = _baseColor;
                    c.a = Mathf.Clamp01(_life / _maxLife);
                    _sr.color = c;
                }
            }
        }
    }
}
