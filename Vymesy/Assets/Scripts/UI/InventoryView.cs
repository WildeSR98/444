using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vymesy.Inventory;

namespace Vymesy.UI
{
    public class InventoryView : MonoBehaviour
    {
        [System.Serializable]
        public class SlotView
        {
            public Image Icon;
            public Image Frame;
            public Text Name;
        }

        [SerializeField] private InventoryManager _inventory;
        [SerializeField] private List<SlotView> _slots = new List<SlotView>();

        private void OnEnable()
        {
            if (_inventory != null) _inventory.OnInventoryChanged += Refresh;
            Refresh();
        }

        private void OnDisable()
        {
            if (_inventory != null) _inventory.OnInventoryChanged -= Refresh;
        }

        private void Refresh()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                var slot = _slots[i];
                if (slot == null) continue;
                ItemData item = (_inventory != null && i < _inventory.Items.Count) ? _inventory.Items[i] : null;
                if (slot.Icon != null) { slot.Icon.enabled = item != null; if (item != null) slot.Icon.sprite = item.Icon; }
                if (slot.Frame != null) slot.Frame.color = item != null ? ItemRarityColors.Color(item.Rarity) : Color.gray;
                if (slot.Name != null) slot.Name.text = item != null ? item.DisplayName : "";
            }
        }
    }
}
