using System.Collections.Generic;
using UnityEngine;
using Vymesy.Inventory;
using Vymesy.Gems;

namespace Vymesy.Player
{
    /// <summary>
    /// Aggregates the player's runtime components, base stats and applied modifiers.
    /// </summary>
    public class PlayerManager : MonoBehaviour
    {
        [SerializeField] private PlayerController _controller;
        [SerializeField] private PlayerHealth _health;
        [SerializeField] private PlayerStats _baseStats = new PlayerStats();

        public PlayerStats Stats { get; private set; }
        public PlayerCurrency Currency { get; } = new PlayerCurrency();
        public PlayerHealth Health => _health;
        public PlayerController Controller => _controller;

        private readonly List<PlayerStatsModifier> _modifiers = new List<PlayerStatsModifier>();

        private void Awake()
        {
            if (_controller == null) _controller = GetComponent<PlayerController>();
            if (_health == null) _health = GetComponent<PlayerHealth>();
            RebuildStats();
        }

        public void ResetForRun()
        {
            Currency.Reset();
            _modifiers.Clear();
            RebuildStats();
            if (_health != null) _health.RestoreFull();
        }

        public void AddModifier(PlayerStatsModifier mod)
        {
            if (mod == null) return;
            _modifiers.Add(mod);
            RebuildStats();
        }

        public void RemoveModifier(PlayerStatsModifier mod)
        {
            if (mod == null) return;
            _modifiers.Remove(mod);
            RebuildStats();
        }

        public void RebuildStats()
        {
            Stats = _baseStats.Clone();
            for (int i = 0; i < _modifiers.Count; i++) Stats.AddFrom(_modifiers[i]);
            if (_controller != null) _controller.Bind(Stats);
            if (_health != null) _health.Bind(Stats);
        }

        public void AddCurrency(CurrencyType type, int amount)
        {
            if (type == CurrencyType.Gold) amount = Mathf.RoundToInt(amount * Stats.GoldMultiplier);
            Currency.Add(type, amount);
        }

        public void ApplyItem(ItemData item)
        {
            if (item == null) return;
            AddModifier(item.Modifier);
        }

        public void ApplyGem(GemData gem, int level)
        {
            if (gem == null) return;
            AddModifier(gem.GetModifier(level));
        }
    }
}
