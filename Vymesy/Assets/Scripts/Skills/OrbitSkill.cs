using UnityEngine;
using Vymesy.Damage;
using Vymesy.Projectiles;

namespace Vymesy.Skills
{
    /// <summary>
    /// Spawns N projectiles that orbit the player.
    /// </summary>
    [CreateAssetMenu(fileName = "OrbitSkill", menuName = "Vymesy/Skills/Orbit Skill")]
    public class OrbitSkill : SkillBase
    {
        public string ProjectilePoolKey = "proj_orbit";
        public int OrbCount = 3;
        public float OrbitRadius = 1.5f;
        public float AngularSpeed = 180f;
        public float Lifetime = 4f;

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.Projectiles == null || ctx.PlayerTransform == null) return;
            int count = Mathf.Max(1, OrbCount + (ctx.Stats != null ? ctx.Stats.ProjectilesBonus : 0));
            float radius = OrbitRadius * (ctx.Stats != null ? ctx.Stats.RangeMultiplier : 1f);
            for (int i = 0; i < count; i++)
            {
                float baseAngle = i * (360f / count);
                var info = DamageSystem.BuildPlayerDamage(BaseDamage, ctx.Stats, DamageType.Physical, Vector2.zero, ctx.Source);
                ctx.Projectiles.FireOrbit(ProjectilePoolKey, ctx.PlayerTransform, baseAngle, radius, AngularSpeed, Lifetime, info);
            }
        }
    }
}
