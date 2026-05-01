using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;

namespace Vymesy.Towers
{
    /// <summary>
    /// Hits the closest enemy within range each tick.
    /// </summary>
    public class CircleTower : TowerBase
    {
        protected override void Tick()
        {
            float bestSqr = Definition.Range * Definition.Range;
            EnemyController best = null;
            Vector2 origin = transform.position;
            foreach (var kv in Enemies.AliveEnemies)
            {
                var ctrl = kv.Value;
                if (ctrl == null || ctrl.Health == null || !ctrl.Health.IsAlive) continue;
                float sqr = ((Vector2)ctrl.transform.position - origin).sqrMagnitude;
                if (sqr <= bestSqr) { bestSqr = sqr; best = ctrl; }
            }
            if (best != null) best.Health.TakeDamage(BuildDamage(Definition.Damage, DamageType.Physical));
        }
    }
}
