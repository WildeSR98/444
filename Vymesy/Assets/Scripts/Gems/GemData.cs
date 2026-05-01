using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Gems
{
    public enum GemStat
    {
        Damage,
        AttackSpeed,
        CritChance,
        MoveSpeed,
        MaxHealth,
        GoldFind,
    }

    [CreateAssetMenu(fileName = "GemData", menuName = "Vymesy/Gems/Gem")]
    public class GemData : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        public GemStat Stat = GemStat.Damage;
        public float ValuePerLevel = 0.05f;
        public Sprite Icon;
        public int MaxLevel = 20;

        /// <summary>Returns a stat modifier for the given gem level.</summary>
        public PlayerStatsModifier GetModifier(int level)
        {
            level = Mathf.Clamp(level, 1, MaxLevel);
            float v = ValuePerLevel * level;
            var mod = new PlayerStatsModifier();
            switch (Stat)
            {
                case GemStat.Damage: mod.DamageMultiplier = v; break;
                case GemStat.AttackSpeed: mod.AttackSpeedMultiplier = v; break;
                case GemStat.CritChance: mod.CritChance = v; break;
                case GemStat.MoveSpeed: mod.MoveSpeed = v; break;
                case GemStat.MaxHealth: mod.MaxHealth = v * 100f; break;
                case GemStat.GoldFind: mod.GoldMultiplier = v; break;
            }
            return mod;
        }
    }
}
