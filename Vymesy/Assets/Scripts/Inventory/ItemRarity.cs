using UnityEngine;

namespace Vymesy.Inventory
{
    public enum ItemRarity
    {
        Normal,
        Rare,
        Epic,
        Legendary,
    }

    public static class ItemRarityColors
    {
        public static Color Color(ItemRarity rarity) => rarity switch
        {
            ItemRarity.Normal => new Color(0.85f, 0.85f, 0.85f),
            ItemRarity.Rare => new Color(0.4f, 0.6f, 1f),
            ItemRarity.Epic => new Color(0.7f, 0.4f, 1f),
            ItemRarity.Legendary => new Color(1f, 0.65f, 0.1f),
            _ => UnityEngine.Color.white,
        };
    }
}
