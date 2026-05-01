using System;

namespace Vymesy.Skills
{
    [Flags]
    public enum SkillTag
    {
        None = 0,
        Projectile = 1 << 0,
        AoE = 1 << 1,
        Chain = 1 << 2,
        Buff = 1 << 3,
        Debuff = 1 << 4,
        Orbit = 1 << 5,
        Holy = 1 << 6,
        Shadow = 1 << 7,
    }
}
