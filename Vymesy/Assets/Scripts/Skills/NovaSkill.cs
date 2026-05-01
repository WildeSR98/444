using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;

namespace Vymesy.Skills
{
    /// <summary>
    /// Expanding shockwave nova: damages enemies once as the ring sweeps past their position.
    /// Visualises with a runtime line-renderer ring.
    /// </summary>
    [CreateAssetMenu(menuName = "Vymesy/Skills/Nova Skill", fileName = "NovaSkill")]
    public class NovaSkill : SkillBase
    {
        [Header("Nova")]
        public float ExpansionSpeed = 8f;
        public float MaxRadius = 6f;
        public float RingThickness = 0.6f;
        public Color RingColor = new Color(0.95f, 0.7f, 0.2f);

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.Enemies == null || ctx.PlayerTransform == null) return;
            var ring = NovaRing.Spawn(ctx.PlayerTransform, MaxRadius, ExpansionSpeed, RingThickness, RingColor);
            ring.Bind(BaseDamage, ctx.Enemies.AliveEnemies, ctx.Stats, ctx.Source);
        }

        private class NovaRing : MonoBehaviour
        {
            private Transform _origin;
            private float _radius;
            private float _maxRadius;
            private float _speed;
            private float _thickness;
            private Color _color;
            private float _baseDamage;
            private System.Collections.Generic.IReadOnlyDictionary<int, EnemyController> _enemies;
            private Player.PlayerStats _stats;
            private GameObject _source;
            private System.Collections.Generic.HashSet<EnemyHealth> _hit;
            private LineRenderer _lr;

            public static NovaRing Spawn(Transform origin, float maxRadius, float speed, float thickness, Color color)
            {
                var go = new GameObject("NovaRing");
                go.transform.position = origin.position;
                var lr = go.AddComponent<LineRenderer>();
                lr.loop = true;
                lr.material = new Material(Shader.Find("Sprites/Default"));
                lr.positionCount = 32;
                lr.widthMultiplier = thickness;
                lr.startColor = color;
                lr.endColor = new Color(color.r, color.g, color.b, 0.2f);
                lr.sortingOrder = 7;
                var ring = go.AddComponent<NovaRing>();
                ring._origin = origin;
                ring._maxRadius = maxRadius;
                ring._speed = speed;
                ring._thickness = thickness;
                ring._color = color;
                ring._lr = lr;
                ring._hit = new System.Collections.Generic.HashSet<EnemyHealth>();
                return ring;
            }

            public void Bind(float damage, System.Collections.Generic.IReadOnlyDictionary<int, EnemyController> enemies, Player.PlayerStats stats, GameObject source)
            {
                _baseDamage = damage;
                _enemies = enemies;
                _stats = stats;
                _source = source;
            }

            private void Update()
            {
                _radius += _speed * Time.deltaTime;
                if (_origin != null) transform.position = _origin.position;

                int n = _lr.positionCount;
                for (int i = 0; i < n; i++)
                {
                    float a = (i / (float)n) * Mathf.PI * 2f;
                    _lr.SetPosition(i, transform.position + new Vector3(Mathf.Cos(a) * _radius, Mathf.Sin(a) * _radius, 0f));
                }

                if (_enemies != null)
                {
                    foreach (var kv in _enemies)
                    {
                        var ec = kv.Value;
                        if (ec == null || ec.Health == null || !ec.Health.IsAlive) continue;
                        if (_hit.Contains(ec.Health)) continue;
                        float dist = Vector2.Distance(transform.position, ec.transform.position);
                        if (Mathf.Abs(dist - _radius) <= _thickness * 0.6f)
                        {
                            var info = DamageSystem.BuildPlayerDamage(_baseDamage, _stats ?? new Player.PlayerStats(), DamageType.Holy, Vector2.zero, _source);
                            ec.Health.TakeDamage(info);
                            _hit.Add(ec.Health);
                        }
                    }
                }

                if (_radius >= _maxRadius) Destroy(gameObject);
            }
        }
    }
}
