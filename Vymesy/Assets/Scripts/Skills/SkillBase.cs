using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Skills
{
    public abstract class SkillBase : ScriptableObject
    {
        [Header("Identity")]
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;

        [Header("Cooldown")]
        public float Cooldown = 1f;
        [Range(0f, 1f)] public float CooldownVariance = 0.05f;

        [Header("Combat")]
        public float BaseDamage = 5f;
        public SkillTag Tags = SkillTag.Projectile;

        public virtual float ResolveCooldown(PlayerStats stats)
        {
            float speed = Mathf.Max(0.1f, stats != null ? stats.AttackSpeedMultiplier : 1f);
            float cd = Cooldown / speed;
            if (CooldownVariance > 0f) cd *= Random.Range(1f - CooldownVariance, 1f + CooldownVariance);
            return cd;
        }

        /// <summary>
        /// Trigger the skill. Implementations should be fully self-contained — the caller
        /// passes everything needed and does not assume side-effects beyond the SkillsManager.
        /// </summary>
        public abstract void Trigger(SkillContext ctx);
    }

    public struct SkillContext
    {
        public Transform PlayerTransform;
        public PlayerStats Stats;
        public Projectiles.ProjectilesManager Projectiles;
        public Enemies.EnemiesManager Enemies;
        public GameObject Source;
    }
}
