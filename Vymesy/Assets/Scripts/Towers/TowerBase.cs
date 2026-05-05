using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;
using Vymesy.Player;

namespace Vymesy.Towers
{
    public abstract class TowerBase : MonoBehaviour
    {
        protected TowerDefinition Definition;
        protected EnemiesManager Enemies;
        protected PlayerManager Player;
        protected float NextTickTime;

        public void Configure(TowerDefinition def, EnemiesManager enemies, PlayerManager player)
        {
            Definition = def;
            Enemies = enemies;
            Player = player;
            NextTickTime = 0f;
        }

        protected virtual void Update()
        {
            if (Definition == null || Enemies == null) return;
            if (Time.time < NextTickTime) return;
            NextTickTime = Time.time + Definition.TickInterval;
            Tick();
        }

        protected abstract void Tick();

        protected DamageInfo BuildDamage(float amount, DamageType type, Vector2 knock = default)
        {
            return new DamageInfo
            {
                Amount = amount,
                Type = type,
                IsCritical = false,
                Knockback = knock,
                Source = gameObject,
            };
        }
    }
}
