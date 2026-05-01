using System;
using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Player;

namespace Vymesy.Gems
{
    public class GemsManager : MonoBehaviour
    {
        [SerializeField] private int _slotCount = 4;
        [SerializeField] private List<GemData> _gemCatalog = new List<GemData>();

        private readonly List<GemSlot> _slots = new List<GemSlot>();
        private readonly List<PlayerStatsModifier> _appliedModifiers = new List<PlayerStatsModifier>();
        public IReadOnlyList<GemSlot> Slots => _slots;
        public IReadOnlyList<GemData> Catalog => _gemCatalog;
        public event Action OnGemsChanged;

        private void Awake()
        {
            for (int i = 0; i < _slotCount; i++) _slots.Add(new GemSlot());
        }

        public void BeginRun() => ApplyAll();
        public void EndRun() { /* gems persist across runs through PlayerData */ }

        public bool Equip(int slot, GemData gem, int level = 1)
        {
            if (slot < 0 || slot >= _slots.Count || gem == null) return false;
            Unequip(slot);
            _slots[slot] = new GemSlot { Gem = gem, Level = Mathf.Clamp(level, 1, gem.MaxLevel) };
            ApplyAll();
            OnGemsChanged?.Invoke();
            return true;
        }

        public void Unequip(int slot)
        {
            if (slot < 0 || slot >= _slots.Count) return;
            _slots[slot] = new GemSlot();
            ApplyAll();
            OnGemsChanged?.Invoke();
        }

        public bool LevelUp(int slot)
        {
            if (slot < 0 || slot >= _slots.Count) return false;
            var s = _slots[slot];
            if (s.IsEmpty) return false;
            if (s.Level >= s.Gem.MaxLevel) return false;
            s.Level += 1;
            ApplyAll();
            OnGemsChanged?.Invoke();
            return true;
        }

        /// <summary>Reroll the gem in the slot to a different gem from the catalog.</summary>
        public bool Reroll(int slot)
        {
            if (slot < 0 || slot >= _slots.Count) return false;
            if (_gemCatalog.Count == 0) return false;
            var s = _slots[slot];
            int level = s.IsEmpty ? 1 : s.Level;
            GemData candidate = null;
            for (int attempts = 0; attempts < 5; attempts++)
            {
                candidate = _gemCatalog[UnityEngine.Random.Range(0, _gemCatalog.Count)];
                if (candidate != null && (s.IsEmpty || candidate != s.Gem)) break;
            }
            if (candidate == null) return false;
            return Equip(slot, candidate, level);
        }

        public void ApplyAll()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null || rm.Player == null) return;
            var pm = rm.Player;

            // Remove previously-applied gem modifiers so repeated ApplyAll calls don't compound.
            for (int i = 0; i < _appliedModifiers.Count; i++) pm.RemoveModifier(_appliedModifiers[i]);
            _appliedModifiers.Clear();

            foreach (var s in _slots)
            {
                if (s == null || s.IsEmpty) continue;
                var mod = s.Gem.GetModifier(s.Level);
                _appliedModifiers.Add(mod);
                pm.AddModifier(mod);
            }
        }
    }
}
