using System.Collections.Generic;
using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Skills
{
    /// <summary>
    /// Generates a small batch of <see cref="SkillUpgradeOffer"/>s tailored to the player's
    /// currently equipped skills + a couple of player stat options. Pure logic with no
    /// scene dependencies.
    /// </summary>
    public static class SkillUpgradeFactory
    {
        public static List<SkillUpgradeOffer> Generate(SkillsManager skills, PlayerManager player, IReadOnlyList<SkillBase> unlockedPool, int count = 3)
        {
            var pool = new List<SkillUpgradeOffer>();

            if (skills != null)
            {
                foreach (var s in skills.EquippedSkills)
                {
                    if (s == null) continue;
                    pool.Add(new SkillUpgradeOffer
                    {
                        DisplayName = $"{s.DisplayName}: +20% урон",
                        Description = $"BaseDamage × 1.20",
                        Apply = () => s.BaseDamage *= 1.20f,
                        Kind = SkillUpgradeOffer.OfferKind.Damage,
                        SkillRef = s,
                    });

                    pool.Add(new SkillUpgradeOffer
                    {
                        DisplayName = $"{s.DisplayName}: −12% кулдаун",
                        Description = $"Cooldown × 0.88",
                        Apply = () => s.Cooldown = Mathf.Max(0.05f, s.Cooldown * 0.88f),
                        Kind = SkillUpgradeOffer.OfferKind.Cooldown,
                        SkillRef = s,
                    });

                    if (s is ProjectileSkill ps)
                    {
                        pool.Add(new SkillUpgradeOffer
                        {
                            DisplayName = $"{s.DisplayName}: +1 снаряд",
                            Description = $"BaseProjectiles + 1",
                            Apply = () => ps.BaseProjectiles += 1,
                            Kind = SkillUpgradeOffer.OfferKind.Projectiles,
                            SkillRef = s,
                        });
                    }

                    if (s is HomingSkill hs)
                    {
                        pool.Add(new SkillUpgradeOffer
                        {
                            DisplayName = $"{s.DisplayName}: +1 ракета",
                            Description = $"Missiles + 1",
                            Apply = () => hs.Missiles += 1,
                            Kind = SkillUpgradeOffer.OfferKind.Projectiles,
                            SkillRef = s,
                        });
                    }

                    if (s is ChainSkill cs)
                    {
                        pool.Add(new SkillUpgradeOffer
                        {
                            DisplayName = $"{s.DisplayName}: +1 прыжок",
                            Description = $"Bounces + 1",
                            Apply = () => cs.Bounces += 1,
                            Kind = SkillUpgradeOffer.OfferKind.Range,
                            SkillRef = s,
                        });
                    }
                }
            }

            // Offer to equip an unlocked skill if there's free space.
            if (skills != null && skills.EquippedSkills.Count < skills.MaxEquipped && unlockedPool != null)
            {
                foreach (var candidate in unlockedPool)
                {
                    if (candidate == null) continue;
                    if (IsEquipped(skills, candidate)) continue;
                    var c = candidate;
                    pool.Add(new SkillUpgradeOffer
                    {
                        DisplayName = $"Новый: {c.DisplayName}",
                        Description = c.Description,
                        Apply = () => skills.Equip(c),
                        Kind = SkillUpgradeOffer.OfferKind.NewSkill,
                        SkillRef = c,
                    });
                }
            }

            // Generic player stats.
            if (player != null)
            {
                pool.Add(new SkillUpgradeOffer
                {
                    DisplayName = "Жажда жизни: +20 HP",
                    Description = "Игрок получает +20 максимальных HP и полностью восстанавливается.",
                    Apply = () => { var mod = new PlayerStatsModifier { MaxHealth = 20 }; player.AddModifier(mod); player.Health?.RestoreFull(); },
                    Kind = SkillUpgradeOffer.OfferKind.PlayerStat,
                });
                pool.Add(new SkillUpgradeOffer
                {
                    DisplayName = "Тёмный шаг: +10% скорость",
                    Description = "+0.5 к MoveSpeed.",
                    Apply = () => { var mod = new PlayerStatsModifier { MoveSpeed = 0.5f }; player.AddModifier(mod); },
                    Kind = SkillUpgradeOffer.OfferKind.PlayerStat,
                });
                pool.Add(new SkillUpgradeOffer
                {
                    DisplayName = "Удар правды: +5% крит",
                    Description = "+0.05 к CritChance.",
                    Apply = () => { var mod = new PlayerStatsModifier { CritChance = 0.05f }; player.AddModifier(mod); },
                    Kind = SkillUpgradeOffer.OfferKind.PlayerStat,
                });
                pool.Add(new SkillUpgradeOffer
                {
                    DisplayName = "Алчность: +25% золото",
                    Description = "+0.25 к GoldMultiplier.",
                    Apply = () => { var mod = new PlayerStatsModifier { GoldMultiplier = 0.25f }; player.AddModifier(mod); },
                    Kind = SkillUpgradeOffer.OfferKind.PlayerStat,
                });
            }

            // Shuffle and take `count`.
            var result = new List<SkillUpgradeOffer>(count);
            var indices = new List<int>(pool.Count);
            for (int i = 0; i < pool.Count; i++) indices.Add(i);
            for (int i = 0; i < count && indices.Count > 0; i++)
            {
                int p = Random.Range(0, indices.Count);
                result.Add(pool[indices[p]]);
                indices.RemoveAt(p);
            }
            return result;
        }

        private static bool IsEquipped(SkillsManager skills, SkillBase candidate)
        {
            for (int i = 0; i < skills.EquippedSkills.Count; i++)
                if (skills.EquippedSkills[i] == candidate) return true;
            return false;
        }
    }
}
