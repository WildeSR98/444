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
        private bool _running;

        public IReadOnlyList<SkillBase> EquippedSkills => _equippedSkills;
        public int MaxEquipped => _maxEquipped;

        public bool Equip(SkillBase skill)
        {
            if (skill == null) return false;
            if (_equippedSkills.Contains(skill)) return false;
            if (_equippedSkills.Count >= _maxEquipped) return false;
            _equippedSkills.Add(skill);
            _cooldowns[skill] = 0f;
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
            foreach (var s in _equippedSkills) _cooldowns[s] = 0f;
        }

        public void EndRun() => _running = false;

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
