using UnityEngine;
using Vymesy.Player;

namespace Vymesy.Inventory
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Vymesy/Items/Item")]
    public class ItemData : ScriptableObject
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public ItemRarity Rarity = ItemRarity.Normal;
        public Sprite Icon;
        public int SellGold = 5;
        public PlayerStatsModifier Modifier = new PlayerStatsModifier();
    }
}
