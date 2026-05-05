using System.Collections.Generic;
using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Projectiles;

namespace Vymesy.Skills
{
    /// <summary>
    /// Fires N homing missiles toward the closest enemies. Each missile pierces 0 enemies
    /// by default but turns to follow its target.
    /// </summary>
    [CreateAssetMenu(menuName = "Vymesy/Skills/Homing Skill", fileName = "HomingSkill")]
    public class HomingSkill : SkillBase
    {
        [Header("Homing")]
        public string ProjectilePoolKey = "proj_homing";
        public int Missiles = 3;
        public float ProjectileSpeed = 8f;
        public float Range = 12f;

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.Projectiles == null || ctx.Enemies == null) return;
            var targets = PickTargets(ctx.Enemies.AliveEnemies, ctx.PlayerTransform.position, Range, Missiles);

            for (int i = 0; i < Missiles; i++)
            {
                Vector2 dir;
                if (i < targets.Count)
                {
                    dir = ((Vector2)targets[i].transform.position - (Vector2)ctx.PlayerTransform.position).normalized;
                }
                else
                {
                    float a = i * (Mathf.PI * 2f / Mathf.Max(1, Missiles));
                    dir = new Vector2(Mathf.Cos(a), Mathf.Sin(a));
                }
                var info = DamageSystem.BuildPlayerDamage(BaseDamage, ctx.Stats ?? new Vymesy.Player.PlayerStats(), DamageType.Physical, dir, ctx.Source);
                ctx.Projectiles.Fire(ProjectilePoolKey, ctx.PlayerTransform.position, dir, ProjectileSpeed, Range, info);
            }
        }

        private static List<EnemyHealth> PickTargets(IReadOnlyDictionary<int, EnemyController> enemies, Vector3 origin, float maxRange, int count)
        {
            var list = new List<(float Dist, EnemyHealth Health)>(count);
            float maxSqr = maxRange * maxRange;
            foreach (var kv in enemies)
            {
                var c = kv.Value;
                if (c == null || c.Health == null || !c.Health.IsAlive) continue;
                float d = ((Vector2)(c.transform.position - origin)).sqrMagnitude;
                if (d > maxSqr) continue;
                list.Add((d, c.Health));
            }
            list.Sort((a, b) => a.Dist.CompareTo(b.Dist));
            var result = new List<EnemyHealth>(Mathf.Min(count, list.Count));
            for (int i = 0; i < count && i < list.Count; i++) result.Add(list[i].Health);
            return result;
        }
    }
}
