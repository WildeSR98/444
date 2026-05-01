using UnityEngine;

namespace Vymesy.Damage
{
    public enum DamageType { Physical, Fire, Poison, Holy, True }

    public struct DamageInfo
    {
        public float Amount;
        public DamageType Type;
        public bool IsCritical;
        public Vector2 Knockback;
        public GameObject Source;

        public static DamageInfo Physical(float amount, GameObject source = null, bool crit = false, Vector2 knockback = default)
        {
            return new DamageInfo
            {
                Amount = amount,
                Type = DamageType.Physical,
                IsCritical = crit,
                Knockback = knockback,
                Source = source,
            };
        }
    }
}
