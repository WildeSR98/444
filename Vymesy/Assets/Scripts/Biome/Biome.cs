using UnityEngine;

namespace Vymesy.Biome
{
    /// <summary>
    /// Pure data describing a biome — palette, ambient mood, list of hazard archetypes
    /// and their spawn frequency. Created at runtime by <see cref="BiomeManager"/>.
    /// </summary>
    [System.Serializable]
    public class Biome
    {
        public string Id;
        public string DisplayName;
        public Color BackgroundColor = new Color(0.04f, 0.03f, 0.06f);
        public Color FogTint = new Color(0.2f, 0.2f, 0.3f, 0.4f);
        public Color HazardColor = new Color(1f, 0.4f, 0.2f);
        public BiomeHazard HazardKind = BiomeHazard.Lava;
        public int HazardCount = 4;
        public float HazardRadius = 1.4f;
        public float HazardDamage = 4f;
        public float EnemyHealthMultiplier = 1f;
        public float EnemySpeedMultiplier = 1f;
    }

    public enum BiomeHazard
    {
        None,
        Lava,
        Fog,
        Ice,
        ShadowVent,
    }
}
