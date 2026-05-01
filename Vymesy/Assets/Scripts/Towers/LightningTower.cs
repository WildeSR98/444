using System.Collections.Generic;
using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;

namespace Vymesy.Towers
{
    /// <summary>
    /// Each tick fires a chain of lightning that hops up to N enemies in range.
    /// </summary>
    public class LightningTower : TowerBase
    {
        [SerializeField] private int _chainCount = 3;
        [SerializeField] private float _jumpRange = 3f;
        [SerializeField] private float _falloffPerJump = 0.2f;

        protected override void Tick()
        {
            if (Enemies == null || Definition == null) return;
            var enemies = Enemies.AliveEnemies;
            var initial = FindClosest(enemies, transform.position, Definition.Range);
            if (initial == null) return;

            float dmg = Definition.Damage;
            var visited = new HashSet<EnemyHealth>();
            Vector3 from = transform.position;
            var current = initial;
            for (int i = 0; i <= _chainCount && current != null; i++)
            {
                current.TakeDamage(BuildDamage(dmg, DamageType.Physical));
                visited.Add(current);
                DrawBolt(from, current.transform.position);
                from = current.transform.position;
                dmg *= 1f - _falloffPerJump;
                current = FindClosestExcluding(enemies, from, _jumpRange, visited);
            }
        }

        private static EnemyHealth FindClosest(IReadOnlyDictionary<int, EnemyController> enemies, Vector3 origin, float maxRange)
        {
            EnemyHealth best = null;
            float bestSqr = maxRange * maxRange;
            foreach (var kv in enemies)
            {
                var c = kv.Value;
                if (c == null || c.Health == null || !c.Health.IsAlive) continue;
                float d = ((Vector2)(c.transform.position - origin)).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = c.Health; }
            }
            return best;
        }

        private static EnemyHealth FindClosestExcluding(IReadOnlyDictionary<int, EnemyController> enemies, Vector3 origin, float maxRange, HashSet<EnemyHealth> exclude)
        {
            EnemyHealth best = null;
            float bestSqr = maxRange * maxRange;
            foreach (var kv in enemies)
            {
                var c = kv.Value;
                if (c == null || c.Health == null || !c.Health.IsAlive) continue;
                if (exclude.Contains(c.Health)) continue;
                float d = ((Vector2)(c.transform.position - origin)).sqrMagnitude;
                if (d < bestSqr) { bestSqr = d; best = c.Health; }
            }
            return best;
        }

        private static void DrawBolt(Vector3 a, Vector3 b)
        {
            var go = new GameObject("TowerBolt");
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPositions(new[] { a, b });
            lr.widthMultiplier = 0.06f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = new Color(0.6f, 0.8f, 1f, 0.95f);
            lr.endColor = new Color(0.6f, 0.8f, 1f, 0.2f);
            lr.sortingOrder = 8;
            Object.Destroy(go, 0.18f);
        }
    }
}
