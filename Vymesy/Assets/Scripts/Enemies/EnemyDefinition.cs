using UnityEngine;

namespace Vymesy.Enemies
{
    [CreateAssetMenu(fileName = "EnemyDefinition", menuName = "Vymesy/Enemies/Enemy Definition")]
    public class EnemyDefinition : ScriptableObject
    {
        public EnemyType Type = EnemyType.Common;
        public string DisplayName = "Вымес";

        [Header("Stats")]
        public float MaxHealth = 20f;
        public float MoveSpeed = 2f;
        public float ContactDamage = 5f;
        public float AttackInterval = 1f;

        [Header("Ranged (optional)")]
        public bool IsRanged = false;
        public float ProjectileSpeed = 5f;
        public float AttackRange = 6f;
        public string ProjectilePoolKey;

        [Header("Loot")]
        public int GoldDrop = 1;
        [Range(0f, 1f)] public float ItemDropChance = 0.02f;
        [Range(0f, 1f)] public float SoulShardDropChance = 0f;

        [Header("Visual")]
        public Sprite Sprite;
        public Color Tint = Color.white;
        public Vector2 ColliderSize = new Vector2(0.6f, 0.6f);
    }
}
