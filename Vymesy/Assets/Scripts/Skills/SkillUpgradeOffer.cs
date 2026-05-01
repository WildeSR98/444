using System;

namespace Vymesy.Skills
{
    /// <summary>
    /// Single choice in the level-up modal. The action mutates the skill's runtime state
    /// (or the SkillsManager), and the modal applies it when the player picks the offer.
    /// </summary>
    public class SkillUpgradeOffer
    {
        public string DisplayName;
        public string Description;
        public Action Apply;
        public OfferKind Kind;
        public SkillBase SkillRef;

        public enum OfferKind
        {
            Damage,
            Cooldown,
            Projectiles,
            Range,
            NewSkill,
            PlayerStat,
        }
    }
}
