using UnityEngine;
using Vymesy.Pooling;
using Vymesy.Utils;

namespace Vymesy.Enemies
{
    /// <summary>
    /// Aggregates the enemy's runtime components and ties them to the manager / pool.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    [RequireComponent(typeof(EnemyAI))]
    public class EnemyController : MonoBehaviour, IPooled
    {
        [SerializeField] private EnemyHealth _health;
        [SerializeField] private EnemyAI _ai;
        [SerializeField] private EnemyView _view;

        private EnemyDefinition _definition;
        private EnemiesManager _manager;
        private string _poolKey;
        private bool _registered;

        public EnemyDefinition Definition => _definition;
        public EnemyHealth Health => _health;

        private void Reset()
        {
            _health = GetComponent<EnemyHealth>();
            _ai = GetComponent<EnemyAI>();
            _view = GetComponent<EnemyView>();
        }

        private void Awake()
        {
            if (_health == null) _health = GetComponent<EnemyHealth>();
            if (_ai == null) _ai = GetComponent<EnemyAI>();
            if (_view == null) _view = GetComponent<EnemyView>();
        }

        public void Configure(EnemyDefinition def, EnemiesManager manager, string poolKey, Transform target, float difficultyMultiplier)
        {
            _definition = def;
            _manager = manager;
            _poolKey = poolKey;
            _health.Reset(def.MaxHealth * Mathf.Max(0.1f, difficultyMultiplier));
            _ai.Initialize(def, target, difficultyMultiplier);
            if (_view != null) _view.Apply(def);

            if (!_registered)
            {
                _registered = true;
                _health.OnDamaged += HandleDamaged;
                _health.OnDied += HandleDied;
            }
        }

        public void OnSpawnedFromPool() { }

        public void OnReturnedToPool()
        {
            // Cleared each return so we don't accumulate listeners.
            _health.OnDamaged -= HandleDamaged;
            _health.OnDied -= HandleDied;
            _registered = false;
        }

        private void HandleDamaged(Damage.DamageInfo info)
        {
            if (_view != null) _view.Flash();
        }

        private void HandleDied()
        {
            if (_manager != null) _manager.OnEnemyDied(this, _poolKey);
            int gold = _definition != null ? Mathf.Max(1, _definition.GoldDrop) : 1;
            EventBus.Publish(new EnemyKilledEvent(_definition != null ? _definition.Type : EnemyType.Common, transform.position, gold));
        }
    }
}
