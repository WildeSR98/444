using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Enemies;
using Vymesy.Player;
using Vymesy.Projectiles;

namespace Vymesy.Skills
{
    /// <summary>
    /// Manages the player's equipped skills, ticks cooldowns, and triggers them.
    /// </summary>
    public class SkillsManager : MonoBehaviour
    {
        [SerializeField] private List<SkillBase> _equippedSkills = new List<SkillBase>();
        [SerializeField] private int _maxEquipped = 6;

        private readonly Dictionary<SkillBase, float> _cooldowns = new Dictionary<SkillBase, float>();
        // Per-skill snapshot of mutable SO fields captured on Equip / BeginRun. Run-scoped
        // upgrades (level-up modal, etc.) mutate the ScriptableObject in place; without this
        // snapshot the changes would compound across runs (e.g. BaseDamage doubles every
        // run instead of resetting to the original value at run boundaries).
        private readonly Dictionary<SkillBase, SkillFieldSnapshot> _originalFields = new Dictionary<SkillBase, SkillFieldSnapshot>();
        private bool _running;

        private struct SkillFieldSnapshot
        {
            public float BaseDamage;
            public float Cooldown;
            public bool HasProjectile; public int BaseProjectiles;
            public bool HasHoming; public int Missiles;
            public bool HasChain; public int Bounces;
        }

        public IReadOnlyList<SkillBase> EquippedSkills => _equippedSkills;
        public int MaxEquipped => _maxEquipped;

        public bool Equip(SkillBase skill)
        {
            if (skill == null) return false;
            if (_equippedSkills.Contains(skill)) return false;
            if (_equippedSkills.Count >= _maxEquipped) return false;
            _equippedSkills.Add(skill);
            _cooldowns[skill] = 0f;
            CaptureOriginal(skill);
            return true;
        }

        public bool Unequip(SkillBase skill)
        {
            if (skill == null) return false;
            _cooldowns.Remove(skill);
            return _equippedSkills.Remove(skill);
        }

        public void BeginRun()
        {
            _running = true;
            _cooldowns.Clear();
            foreach (var s in _equippedSkills)
            {
                _cooldowns[s] = 0f;
                CaptureOriginal(s);
            }
        }

        public void EndRun()
        {
            _running = false;
            // Restore SO fields mutated by run-scoped level-up upgrades (SkillUpgradeFactory)
            // so the next run starts from baseline values instead of compounding indefinitely.
            RestoreOriginals();
            // Cancel any still-running BuffSkill timers and strip their modifiers — otherwise
            // a coroutine started in this run would resume in the next run and remove a
            // freshly-applied buff prematurely.
            BuffSkill.StopAllActive();
        }

        private void CaptureOriginal(SkillBase skill)
        {
            if (skill == null) return;
            if (_originalFields.ContainsKey(skill)) return;
            var snap = new SkillFieldSnapshot
            {
                BaseDamage = skill.BaseDamage,
                Cooldown = skill.Cooldown,
            };
            if (skill is ProjectileSkill ps) { snap.HasProjectile = true; snap.BaseProjectiles = ps.BaseProjectiles; }
            if (skill is HomingSkill hs) { snap.HasHoming = true; snap.Missiles = hs.Missiles; }
            if (skill is ChainSkill cs) { snap.HasChain = true; snap.Bounces = cs.Bounces; }
            _originalFields[skill] = snap;
        }

        private void RestoreOriginals()
        {
            foreach (var kv in _originalFields)
            {
                var skill = kv.Key;
                if (skill == null) continue;
                var snap = kv.Value;
                skill.BaseDamage = snap.BaseDamage;
                skill.Cooldown = snap.Cooldown;
                if (snap.HasProjectile && skill is ProjectileSkill ps) ps.BaseProjectiles = snap.BaseProjectiles;
                if (snap.HasHoming && skill is HomingSkill hs) hs.Missiles = snap.Missiles;
                if (snap.HasChain && skill is ChainSkill cs) cs.Bounces = snap.Bounces;
            }
        }

        private void Update()
        {
            if (!_running) return;
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null || rm.Player == null) return;

            var ctx = new SkillContext
            {
                PlayerTransform = rm.Player.transform,
                Stats = rm.Player.Stats,
                Projectiles = FindProjectilesManager(),
                Enemies = rm.Enemies,
                Source = rm.Player.gameObject,
            };

            float dt = Time.deltaTime;
            for (int i = 0; i < _equippedSkills.Count; i++)
            {
                var skill = _equippedSkills[i];
                if (skill == null) continue;
                _cooldowns.TryGetValue(skill, out var remaining);
                remaining -= dt;
                if (remaining <= 0f)
                {
                    skill.Trigger(ctx);
                    remaining = skill.ResolveCooldown(ctx.Stats);
                }
                _cooldowns[skill] = remaining;
            }
        }

        private static ProjectilesManager FindProjectilesManager()
        {
            return ProjectilesManager.HasInstance ? ProjectilesManager.Instance : null;
        }
    }
}
