using UnityEngine;
using Vymesy.Damage;

namespace Vymesy.Towers
{
    /// <summary>
    /// Damage-over-time across all enemies in range. Each tick deals partial Damage with Poison type.
    /// </summary>
    public class PoisonTower : TowerBase
    {
        protected override void Tick()
        {
            float r = Definition.Range;
            float sqr = r * r;
            Vector2 origin = transform.position;
            float dmg = Definition.Damage;
            foreach (var kv in Enemies.AliveEnemies)
            {
                var ctrl = kv.Value;
                if (ctrl == null || ctrl.Health == null || !ctrl.Health.IsAlive) continue;
                if (((Vector2)ctrl.transform.position - origin).sqrMagnitude > sqr) continue;
                ctrl.Health.TakeDamage(BuildDamage(dmg, DamageType.Poison));
            }
        }
    }
}
