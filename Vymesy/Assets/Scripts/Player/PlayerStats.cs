using System;
using UnityEngine;

namespace Vymesy.Player
{
    [Serializable]
    public class PlayerStats
    {
        [Header("Vitals")]
        public float MaxHealth = 100f;
        public float HealthRegenPerSecond = 0f;

        [Header("Movement")]
        public float MoveSpeed = 5f;

        [Header("Offense")]
        [Tooltip("Multiplier applied to all outgoing damage.")]
        public float DamageMultiplier = 1f;
        [Range(0f, 1f)] public float CritChance = 0.05f;
        public float CritMultiplier = 2f;

        [Header("Skills")]
        public float AttackSpeedMultiplier = 1f;
        public float RangeMultiplier = 1f;
        public float ProjectileSpeedMultiplier = 1f;
        public int ProjectilesBonus = 0;

        [Header("Defense")]
        [Range(0f, 0.9f)] public float DamageReduction = 0f;
        public float Armor = 0f;

        [Header("Utility")]
        public float PickupRadius = 1.5f;
        public float GoldMultiplier = 1f;
        public float ExpMultiplier = 1f;

        public PlayerStats Clone()
        {
            return (PlayerStats)MemberwiseClone();
        }

        /// <summary>Combine two stat snapshots additively (used for item/gem application).</summary>
        public void AddFrom(PlayerStatsModifier mod)
        {
            if (mod == null) return;
            MaxHealth += mod.MaxHealth;
            HealthRegenPerSecond += mod.HealthRegenPerSecond;
            MoveSpeed += mod.MoveSpeed;
            DamageMultiplier += mod.DamageMultiplier;
            CritChance += mod.CritChance;
            CritMultiplier += mod.CritMultiplier;
            AttackSpeedMultiplier += mod.AttackSpeedMultiplier;
            RangeMultiplier += mod.RangeMultiplier;
            ProjectileSpeedMultiplier += mod.ProjectileSpeedMultiplier;
            ProjectilesBonus += mod.ProjectilesBonus;
            DamageReduction = Mathf.Min(0.9f, DamageReduction + mod.DamageReduction);
            Armor += mod.Armor;
            PickupRadius += mod.PickupRadius;
            GoldMultiplier += mod.GoldMultiplier;
            ExpMultiplier += mod.ExpMultiplier;
        }
    }

    /// <summary>
    /// Additive modifier delta. Defaults to all-zero so unused fields don't accidentally change stats.
    /// </summary>
    [Serializable]
    public class PlayerStatsModifier
    {
        public float MaxHealth;
        public float HealthRegenPerSecond;
        public float MoveSpeed;
        public float DamageMultiplier;
        public float CritChance;
        public float CritMultiplier;
        public float AttackSpeedMultiplier;
        public float RangeMultiplier;
        public float ProjectileSpeedMultiplier;
        public int ProjectilesBonus;
        public float DamageReduction;
        public float Armor;
        public float PickupRadius;
        public float GoldMultiplier;
        public float ExpMultiplier;
    }
}
