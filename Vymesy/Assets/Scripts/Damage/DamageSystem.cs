using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Damage
{
    /// <summary>
    /// Resolves outgoing damage from the player, applying crit and modifiers from PlayerStats.
    /// Pure logic — no Unity scene dependencies beyond reading stats.
    /// </summary>
    public static class DamageSystem
    {
        public static DamageInfo BuildPlayerDamage(float baseAmount, PlayerStats stats, DamageType type = DamageType.Physical, Vector2 knockback = default, GameObject source = null)
        {
            float dmg = baseAmount * stats.DamageMultiplier;
            bool crit = Random.value < stats.CritChance;
            if (crit) dmg *= stats.CritMultiplier;
            return new DamageInfo
            {
                Amount = dmg,
                Type = type,
                IsCritical = crit,
                Knockback = knockback,
                Source = source,
            };
        }

        public static void Apply(IDamageable target, in DamageInfo info)
        {
            if (target == null || !target.IsAlive) return;
            target.TakeDamage(info);
        }
    }
}
