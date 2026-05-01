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
            => Make(amount, DamageType.Physical, source, crit, knockback);

        public static DamageInfo Fire(float amount, GameObject source = null, bool crit = false, Vector2 knockback = default)
            => Make(amount, DamageType.Fire, source, crit, knockback);

        public static DamageInfo Poison(float amount, GameObject source = null, bool crit = false, Vector2 knockback = default)
            => Make(amount, DamageType.Poison, source, crit, knockback);

        public static DamageInfo Holy(float amount, GameObject source = null, bool crit = false, Vector2 knockback = default)
            => Make(amount, DamageType.Holy, source, crit, knockback);

        public static DamageInfo True(float amount, GameObject source = null, bool crit = false, Vector2 knockback = default)
            => Make(amount, DamageType.True, source, crit, knockback);

        private static DamageInfo Make(float amount, DamageType type, GameObject source, bool crit, Vector2 knockback)
        {
            return new DamageInfo
            {
                Amount = amount,
                Type = type,
                IsCritical = crit,
                Knockback = knockback,
                Source = source,
            };
        }
    }
}
