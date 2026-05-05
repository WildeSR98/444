using UnityEngine;
using Vymesy.Damage;

namespace Vymesy.Towers
{
    public class AoETower : TowerBase
    {
        protected override void Tick()
        {
            float r = Definition.Range;
            float sqr = r * r;
            Vector2 origin = transform.position;
            foreach (var kv in Enemies.AliveEnemies)
            {
                var ctrl = kv.Value;
                if (ctrl == null || ctrl.Health == null || !ctrl.Health.IsAlive) continue;
                if (((Vector2)ctrl.transform.position - origin).sqrMagnitude > sqr) continue;
                ctrl.Health.TakeDamage(BuildDamage(Definition.Damage, DamageType.Physical));
            }
        }
    }
}
