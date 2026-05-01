using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Enemies;
using Vymesy.Player;
using Vymesy.Utils;

namespace Vymesy.Inventory
{
    /// <summary>
    /// Subscribes to enemy kill events and drops items / currency at the kill location.
    /// </summary>
    public class LootDropper : MonoBehaviour
    {
        [SerializeField] private List<ItemData> _itemPool = new List<ItemData>();
        [SerializeField, Range(0f, 1f)] private float _baseItemChance = 0.05f;

        private void OnEnable() => EventBus.Subscribe<EnemyKilledEvent>(HandleEnemyKilled);
        private void OnDisable() => EventBus.Unsubscribe<EnemyKilledEvent>(HandleEnemyKilled);

        private void HandleEnemyKilled(EnemyKilledEvent evt)
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null) return;

            float chance = _baseItemChance + RarityBonus(evt.Type);
            if (Random.value < chance && _itemPool.Count > 0 && rm.Inventory != null)
            {
                var picked = PickItemByEnemy(evt.Type);
                if (picked != null) rm.Inventory.TryPickUp(picked);
            }

            if (evt.Type == EnemyType.Elite || evt.Type == EnemyType.Boss)
            {
                rm.Player?.AddCurrency(CurrencyType.SoulShards, evt.Type == EnemyType.Boss ? 5 : 1);
            }
        }

        private static float RarityBonus(EnemyType t) => t switch
        {
            EnemyType.Elite => 0.2f,
            EnemyType.Shiny => 1f,
            EnemyType.Boss => 0.5f,
            _ => 0f,
        };

        private ItemData PickItemByEnemy(EnemyType type)
        {
            if (_itemPool.Count == 0) return null;
            int[] weights = new int[_itemPool.Count];
            for (int i = 0; i < _itemPool.Count; i++)
            {
                var it = _itemPool[i];
                if (it == null) { weights[i] = 0; continue; }
                weights[i] = it.Rarity switch
                {
                    ItemRarity.Normal => type == EnemyType.Shiny ? 0 : 60,
                    ItemRarity.Rare => 25,
                    ItemRarity.Epic => type >= EnemyType.Elite ? 12 : 5,
                    ItemRarity.Legendary => type == EnemyType.Boss || type == EnemyType.Shiny ? 5 : 1,
                    _ => 0,
                };
            }
            int idx = MathUtils.WeightedPick(weights);
            return idx >= 0 ? _itemPool[idx] : null;
        }
    }
}
