using UnityEngine;

namespace Vymesy.Damage
{
    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(in DamageInfo info);
        Transform Transform { get; }
    }
}
