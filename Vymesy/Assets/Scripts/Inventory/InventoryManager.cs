using System;
using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Player;
using Vymesy.Utils;

namespace Vymesy.Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        [SerializeField] private int _slotCount = 6;
        [SerializeField] private bool _autoSellNormalItems = false;

        private readonly List<ItemData> _items = new List<ItemData>();
        public IReadOnlyList<ItemData> Items => _items;
        public int SlotCount => _slotCount;
        public bool AutoSellNormalItems { get => _autoSellNormalItems; set => _autoSellNormalItems = value; }
        public event Action OnInventoryChanged;

        public void BeginRun()
        {
            _items.Clear();
            OnInventoryChanged?.Invoke();
        }

        public void EndRun()
        {
            // Items are run-scoped; persistence is up to the SaveLoad layer.
            _items.Clear();
            OnInventoryChanged?.Invoke();
        }

        public bool TryPickUp(ItemData item)
        {
            if (item == null) return false;
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            var player = rm != null ? rm.Player : null;

            if (_autoSellNormalItems && item.Rarity == ItemRarity.Normal)
            {
                if (player != null) player.AddCurrency(CurrencyType.Gold, item.SellGold);
                EventBus.Publish(new ItemPickedUpEvent(item));
                return true;
            }

            if (_items.Count >= _slotCount)
            {
                // Full inventory: sell the lowest-rarity item if the new one is better, else drop.
                int worstIndex = FindWorstIndex();
                if (worstIndex < 0 || _items[worstIndex].Rarity >= item.Rarity)
                {
                    if (player != null) player.AddCurrency(CurrencyType.Gold, item.SellGold);
                    EventBus.Publish(new ItemPickedUpEvent(item));
                    return true;
                }
                var replaced = _items[worstIndex];
                if (player != null)
                {
                    player.RemoveModifier(replaced.Modifier);
                    player.AddCurrency(CurrencyType.Gold, replaced.SellGold);
                }
                _items[worstIndex] = item;
                if (player != null) player.ApplyItem(item);
            }
            else
            {
                _items.Add(item);
                if (player != null) player.ApplyItem(item);
            }
            EventBus.Publish(new ItemPickedUpEvent(item));
            OnInventoryChanged?.Invoke();
            return true;
        }

        public void SellSlot(int index)
        {
            if (index < 0 || index >= _items.Count) return;
            var item = _items[index];
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            var player = rm != null ? rm.Player : null;
            if (player != null)
            {
                player.RemoveModifier(item.Modifier);
                player.AddCurrency(CurrencyType.Gold, item.SellGold);
            }
            _items.RemoveAt(index);
            OnInventoryChanged?.Invoke();
        }

        private int FindWorstIndex()
        {
            if (_items.Count == 0) return -1;
            int idx = 0;
            for (int i = 1; i < _items.Count; i++)
            {
                if ((int)_items[i].Rarity < (int)_items[idx].Rarity) idx = i;
            }
            return idx;
        }
    }
}
