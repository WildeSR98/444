using System;
using UnityEngine;
using Vymesy.Damage;

namespace Vymesy.Enemies
{
    public class EnemyHealth : MonoBehaviour, IDamageable
    {
        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;
        public Transform Transform => transform;

        public event Action<DamageInfo> OnDamaged;
        public event Action OnDied;

        public void Reset(float maxHealth)
        {
            MaxHealth = maxHealth;
            CurrentHealth = maxHealth;
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive) return;
            CurrentHealth -= info.Amount;
            OnDamaged?.Invoke(info);
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                OnDied?.Invoke();
            }
        }
    }
}
