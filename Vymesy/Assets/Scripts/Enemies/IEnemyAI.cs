using UnityEngine;

namespace Vymesy.Enemies
{
    /// <summary>
    /// Contract every enemy AI variant must satisfy so <see cref="EnemyController"/>
    /// can configure it without knowing the concrete type.
    /// </summary>
    public interface IEnemyAI
    {
        void Initialize(EnemyDefinition def, Transform target, float difficultyMultiplier);
    }
}
