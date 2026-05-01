using UnityEngine;
using Vymesy.Damage;
using Vymesy.Demo;
using Vymesy.Enemies;

namespace Vymesy.VFX
{
    /// <summary>
    /// Spawns small sprite "tick" markers above an enemy on each hit. We deliberately avoid
    /// rendering text so the demo doesn't depend on a TMP font asset; numeric values are
    /// approximated by stacking 1..3 ticks colored by crit/non-crit.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class DamageNumberPopup : MonoBehaviour
    {
        [SerializeField] private float _riseSpeed = 1.4f;
        [SerializeField] private float _life = 0.5f;

        private EnemyHealth _health;

        private void Awake() => _health = GetComponent<EnemyHealth>();

        private void OnEnable()
        {
            if (_health != null) _health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(DamageInfo info)
        {
            int ticks = info.IsCritical ? 3 : (info.Amount > 20f ? 2 : 1);
            Color color = info.IsCritical ? new Color(1f, 0.85f, 0.3f) : new Color(1f, 0.9f, 0.9f);
            for (int i = 0; i < ticks; i++)
            {
                Tick.Spawn(transform.position + new Vector3(i * 0.2f - ticks * 0.1f, 0.5f, 0f), color, _riseSpeed, _life);
            }
        }

        private class Tick : MonoBehaviour
        {
            private float _life;
            private float _maxLife;
            private float _rise;
            private SpriteRenderer _sr;
            private Color _baseColor;

            public static void Spawn(Vector3 origin, Color color, float rise, float life)
            {
                var go = new GameObject("HitTick");
                go.transform.position = origin;
                go.transform.localScale = Vector3.one * 0.2f;
                var sr = go.AddComponent<SpriteRenderer>();
                sr.sprite = DemoSprites.Get(DemoSprites.Shape.Diamond, color, 8);
                sr.sortingOrder = 7;
                var t = go.AddComponent<Tick>();
                t._sr = sr;
                t._baseColor = color;
                t._rise = rise;
                t._maxLife = life;
                t._life = life;
            }

            private void Update()
            {
                _life -= Time.deltaTime;
                if (_life <= 0f) { Destroy(gameObject); return; }
                transform.position += new Vector3(0f, _rise * Time.deltaTime, 0f);
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
