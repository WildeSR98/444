using UnityEngine;
using Vymesy.Damage;

namespace Vymesy.Skills
{
    [CreateAssetMenu(fileName = "ProjectileSkill", menuName = "Vymesy/Skills/Projectile Skill")]
    public class ProjectileSkill : SkillBase
    {
        [Header("Projectile")]
        public string ProjectilePoolKey = "proj_basic";
        public int BaseProjectiles = 1;
        public float SpreadDegrees = 8f;
        public float ProjectileSpeed = 12f;
        public float Range = 8f;

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.Projectiles == null || ctx.PlayerTransform == null) return;
            int count = Mathf.Max(1, BaseProjectiles + (ctx.Stats != null ? ctx.Stats.ProjectilesBonus : 0));
            Vector2 origin = ctx.PlayerTransform.position;

            Vector2 baseDir = FindAimDirection(ctx, origin);
            float speed = ProjectileSpeed * (ctx.Stats != null ? ctx.Stats.ProjectileSpeedMultiplier : 1f);
            float range = Range * (ctx.Stats != null ? ctx.Stats.RangeMultiplier : 1f);

            for (int i = 0; i < count; i++)
            {
                float t = count == 1 ? 0f : ((float)i / (count - 1) - 0.5f);
                float angle = t * SpreadDegrees;
                Vector2 dir = Rotate(baseDir, angle);
                var info = DamageSystem.BuildPlayerDamage(BaseDamage, ctx.Stats, DamageType.Physical, dir * 1f, ctx.Source);
                ctx.Projectiles.Fire(ProjectilePoolKey, origin, dir, speed, range, info);
            }
        }

        private static Vector2 FindAimDirection(SkillContext ctx, Vector2 origin)
        {
            if (ctx.Enemies == null) return Vector2.right;
            float bestSqr = float.MaxValue;
            Vector2 dir = Vector2.right;
            foreach (var kv in ctx.Enemies.AliveEnemies)
            {
                if (kv.Value == null) continue;
                Vector2 to = (Vector2)kv.Value.transform.position - origin;
                float sqr = to.sqrMagnitude;
                if (sqr < bestSqr)
                {
                    bestSqr = sqr;
                    dir = to.normalized;
                }
            }
            return dir.sqrMagnitude < 0.0001f ? Vector2.right : dir;
        }

        private static Vector2 Rotate(Vector2 v, float degrees)
        {
            float rad = degrees * Mathf.Deg2Rad;
            float cos = Mathf.Cos(rad);
            float sin = Mathf.Sin(rad);
            return new Vector2(v.x * cos - v.y * sin, v.x * sin + v.y * cos);
        }
    }
}
