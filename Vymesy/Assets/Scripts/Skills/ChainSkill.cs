using System.Collections.Generic;
using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Player;

namespace Vymesy.Skills
{
    /// <summary>
    /// Chain-lightning style skill: damages an initial target then jumps to nearby enemies.
    /// Each jump applies a damage falloff. Uses a LineRenderer for the visual.
    /// </summary>
    [CreateAssetMenu(menuName = "Vymesy/Skills/Chain Skill", fileName = "ChainSkill")]
    public class ChainSkill : SkillBase
    {
        [Header("Chain")]
        public int Bounces = 3;
        public float JumpRange = 4f;
        public float DamageFalloffPerBounce = 0.2f;
        public Color BoltColor = new Color(0.5f, 0.85f, 1f);
        public float BoltLifetime = 0.18f;

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.Enemies == null) return;
            var enemies = ctx.Enemies.AliveEnemies;
            var target = FindClosest(enemies, ctx.PlayerTransform.position, JumpRange * 1.5f);
            if (target == null) return;

            float falloff = 1f;
            var visited = new HashSet<EnemyHealth>();
            Vector3 from = ctx.PlayerTransform.position;
            for (int i = 0; i <= Bounces && target != null; i++)
            {
                var damage = DamageSystem.BuildPlayerDamage(BaseDamage * falloff, ctx.Stats ?? new PlayerStats(), DamageType.Physical, Vector2.zero, ctx.Source);
                target.TakeDamage(damage);
                visited.Add(target);
                DrawBolt(from, target.transform.position, BoltColor, BoltLifetime);
                from = target.transform.position;
                falloff *= 1f - DamageFalloffPerBounce;
                target = FindClosestExcluding(enemies, from, JumpRange, visited);
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

        private static void DrawBolt(Vector3 a, Vector3 b, Color color, float lifetime)
        {
            var go = new GameObject("ChainBolt");
            var lr = go.AddComponent<LineRenderer>();
            lr.positionCount = 2;
            lr.SetPositions(new[] { a, b });
            lr.widthMultiplier = 0.08f;
            lr.material = new Material(Shader.Find("Sprites/Default"));
            lr.startColor = color;
            lr.endColor = new Color(color.r, color.g, color.b, 0.2f);
            lr.sortingOrder = 8;
            Object.Destroy(go, lifetime);
        }
    }
}
