using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Skills
{
    /// <summary>
    /// Temporary stat buff applied to the player when the skill triggers. Each trigger
    /// clones the configured modifier so multiple stacks of the buff can coexist and
    /// stale "remove" coroutines from a previous run cannot delete a fresh stack.
    /// </summary>
    [CreateAssetMenu(menuName = "Vymesy/Skills/Buff Skill", fileName = "BuffSkill")]
    public class BuffSkill : SkillBase
    {
        [Header("Buff")]
        public PlayerStatsModifier Modifier = new PlayerStatsModifier();
        public float Duration = 4f;

        // Tracked across all live BuffSkill triggers so SkillsManager.EndRun can stop
        // any in-flight coroutines and remove their modifiers. Without this, a coroutine
        // started in run N would survive into run N+1 (PlayerManager is persistent) and
        // remove a freshly-applied modifier prematurely.
        private static readonly List<ActiveBuff> _active = new List<ActiveBuff>();

        private struct ActiveBuff
        {
            public PlayerManager Player;
            public PlayerStatsModifier Modifier;
            public Coroutine Routine;
        }

        public override void Trigger(SkillContext ctx)
        {
            PlayerManager pm = null;
            if (ctx.Source != null) pm = ctx.Source.GetComponentInParent<PlayerManager>();
#if UNITY_2023_1_OR_NEWER
            if (pm == null) pm = UnityEngine.Object.FindFirstObjectByType<PlayerManager>();
#else
            if (pm == null) pm = UnityEngine.Object.FindObjectOfType<PlayerManager>();
#endif
            if (pm == null) return;

            // Clone so each activation owns its own modifier reference.
            var clone = new PlayerStatsModifier
            {
                MaxHealth = Modifier.MaxHealth,
                HealthRegenPerSecond = Modifier.HealthRegenPerSecond,
                MoveSpeed = Modifier.MoveSpeed,
                DamageMultiplier = Modifier.DamageMultiplier,
                CritChance = Modifier.CritChance,
                CritMultiplier = Modifier.CritMultiplier,
                AttackSpeedMultiplier = Modifier.AttackSpeedMultiplier,
                RangeMultiplier = Modifier.RangeMultiplier,
                ProjectileSpeedMultiplier = Modifier.ProjectileSpeedMultiplier,
                ProjectilesBonus = Modifier.ProjectilesBonus,
                DamageReduction = Modifier.DamageReduction,
                Armor = Modifier.Armor,
                PickupRadius = Modifier.PickupRadius,
                GoldMultiplier = Modifier.GoldMultiplier,
                ExpMultiplier = Modifier.ExpMultiplier,
            };
            pm.AddModifier(clone);
            var routine = pm.StartCoroutine(RemoveAfter(pm, clone, Duration));
            _active.Add(new ActiveBuff { Player = pm, Modifier = clone, Routine = routine });
        }

        private static IEnumerator RemoveAfter(PlayerManager pm, PlayerStatsModifier mod, float duration)
        {
            yield return new WaitForSeconds(duration);
            if (pm != null) pm.RemoveModifier(mod);
            // Self-remove from the tracker.
            for (int i = _active.Count - 1; i >= 0; i--)
            {
                if (_active[i].Modifier == mod) { _active.RemoveAt(i); break; }
            }
        }

        /// <summary>
        /// Called by <see cref="SkillsManager.EndRun"/> to cancel all in-flight buffs and
        /// strip their modifiers. Prevents cross-run coroutine bleed.
        /// </summary>
        public static void StopAllActive()
        {
            for (int i = 0; i < _active.Count; i++)
            {
                var b = _active[i];
                if (b.Player != null)
                {
                    if (b.Routine != null) b.Player.StopCoroutine(b.Routine);
                    b.Player.RemoveModifier(b.Modifier);
                }
            }
            _active.Clear();
        }
    }
}
