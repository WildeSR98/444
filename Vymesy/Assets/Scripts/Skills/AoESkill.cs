using UnityEngine;
using Vymesy.Damage;

namespace Vymesy.Skills
{
    [CreateAssetMenu(fileName = "AoESkill", menuName = "Vymesy/Skills/AoE Skill")]
    public class AoESkill : SkillBase
    {
        public float Radius = 3f;
        [Tooltip("Optional knockback away from the player.")] public float KnockbackForce = 0f;

        public override void Trigger(SkillContext ctx)
        {
            if (ctx.PlayerTransform == null || ctx.Enemies == null) return;
            Vector2 origin = ctx.PlayerTransform.position;
            float range = Radius * (ctx.Stats != null ? ctx.Stats.RangeMultiplier : 1f);
            float sqr = range * range;
            foreach (var kv in ctx.Enemies.AliveEnemies)
            {
                var ctrl = kv.Value;
                if (ctrl == null || ctrl.Health == null || !ctrl.Health.IsAlive) continue;
                Vector2 to = (Vector2)ctrl.transform.position - origin;
                if (to.sqrMagnitude > sqr) continue;
                Vector2 knock = KnockbackForce > 0f ? to.normalized * KnockbackForce : Vector2.zero;
                var dmg = DamageSystem.BuildPlayerDamage(BaseDamage, ctx.Stats, DamageType.Physical, knock, ctx.Source);
                ctrl.Health.TakeDamage(dmg);
            }
        }
    }
}
