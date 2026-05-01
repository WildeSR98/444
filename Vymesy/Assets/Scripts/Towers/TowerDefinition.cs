using UnityEngine;

namespace Vymesy.Towers
{
    [CreateAssetMenu(fileName = "TowerDefinition", menuName = "Vymesy/Towers/Tower Definition")]
    public class TowerDefinition : ScriptableObject
    {
        public TowerType Type = TowerType.AoE;
        public string DisplayName;
        public float TickInterval = 1f;
        public float Range = 4f;
        public float Damage = 10f;
        public int GoldPerTick = 1;
        public GameObject Prefab;
    }
}
