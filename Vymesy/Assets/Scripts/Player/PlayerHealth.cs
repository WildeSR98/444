using UnityEngine;
using Vymesy.Damage;
using Vymesy.Utils;

namespace Vymesy.Player
{
    public class PlayerHealth : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _invulnerabilitySeconds = 0.4f;

        public float CurrentHealth { get; private set; }
        public float MaxHealth { get; private set; }
        public bool IsAlive => CurrentHealth > 0f;
        public Transform Transform => transform;

        private PlayerStats _stats;
        private float _invulUntil;

        public void Bind(PlayerStats stats)
        {
            _stats = stats;
            MaxHealth = stats.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public void RestoreFull()
        {
            if (_stats == null) return;
            MaxHealth = _stats.MaxHealth;
            CurrentHealth = MaxHealth;
        }

        public void Heal(float amount)
        {
            if (!IsAlive || amount <= 0f) return;
            CurrentHealth = Mathf.Min(MaxHealth, CurrentHealth + amount);
        }

        public void TakeDamage(in DamageInfo info)
        {
            if (!IsAlive) return;
            if (Time.time < _invulUntil) return;

            float reduction = _stats != null ? _stats.DamageReduction : 0f;
            float armor = _stats != null ? _stats.Armor : 0f;
            float mitigated = Mathf.Max(0f, info.Amount * (1f - reduction) - armor * 0.1f);
            CurrentHealth -= mitigated;
            _invulUntil = Time.time + _invulnerabilitySeconds;
            EventBus.Publish(new PlayerDamagedEvent(mitigated));
            if (CurrentHealth <= 0f)
            {
                CurrentHealth = 0f;
                EventBus.Publish(new PlayerDiedEvent());
            }
        }

        private void Update()
        {
            if (!IsAlive || _stats == null) return;
            if (_stats.HealthRegenPerSecond > 0f && CurrentHealth < MaxHealth)
            {
                Heal(_stats.HealthRegenPerSecond * Time.deltaTime);
            }
        }
    }
}
