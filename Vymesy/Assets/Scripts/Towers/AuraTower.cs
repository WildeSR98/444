using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;

namespace Vymesy.Towers
{
    /// <summary>
    /// Continuously damages every enemy within its aura, scaled by tick interval so
    /// behaviour is similar to AoE but visualised as a static aura ring.
    /// </summary>
    public class AuraTower : TowerBase
    {
        [SerializeField] private Color _ringColor = new Color(0.7f, 0.9f, 1f, 0.5f);
        private LineRenderer _ring;

        protected override void Tick()
        {
            if (Enemies == null || Definition == null) return;
            EnsureRing();
            float r = Definition.Range;
            foreach (var kv in Enemies.AliveEnemies)
            {
                var ec = kv.Value;
                if (ec == null || ec.Health == null || !ec.Health.IsAlive) continue;
                if (((Vector2)(ec.transform.position - transform.position)).sqrMagnitude > r * r) continue;
                ec.Health.TakeDamage(BuildDamage(Definition.Damage, DamageType.Holy));
            }
        }

        private void EnsureRing()
        {
            if (_ring != null) return;
            _ring = gameObject.AddComponent<LineRenderer>();
            _ring.loop = true;
            _ring.positionCount = 32;
            _ring.widthMultiplier = 0.05f;
            _ring.material = new Material(Shader.Find("Sprites/Default"));
            _ring.startColor = _ringColor;
            _ring.endColor = _ringColor;
            _ring.sortingOrder = 1;
            float r = Definition != null ? Definition.Range : 3f;
            for (int i = 0; i < _ring.positionCount; i++)
            {
                float a = (i / (float)_ring.positionCount) * Mathf.PI * 2f;
                _ring.SetPosition(i, new Vector3(Mathf.Cos(a) * r, Mathf.Sin(a) * r, 0f));
            }
            _ring.useWorldSpace = false;
        }
    }
}
