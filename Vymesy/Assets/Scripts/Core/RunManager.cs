using System;
using UnityEngine;
using Vymesy.Enemies;
using Vymesy.Player;
using Vymesy.Skills;
using Vymesy.Towers;
using Vymesy.Inventory;
using Vymesy.Gems;
using Vymesy.Save;
using Vymesy.Utils;

namespace Vymesy.Core
{
    /// <summary>
    /// Owns the lifecycle of a single run (start → playing → end).
    /// </summary>
    public class RunManager : MonoBehaviour
    {
        [Header("Scene References")]
        [SerializeField] private PlayerManager _playerManagerPrefab;
        [SerializeField] private EnemiesManager _enemiesManagerPrefab;
        [SerializeField] private SkillsManager _skillsManagerPrefab;
        [SerializeField] private TowersManager _towersManagerPrefab;
        [SerializeField] private InventoryManager _inventoryManagerPrefab;
        [SerializeField] private GemsManager _gemsManagerPrefab;

        public PlayerManager Player { get; private set; }
        public EnemiesManager Enemies { get; private set; }
        public SkillsManager Skills { get; private set; }
        public TowersManager Towers { get; private set; }
        public InventoryManager Inventory { get; private set; }
        public GemsManager Gems { get; private set; }

        public bool IsRunStarted { get; private set; }
        public float RunTime { get; private set; }
        public int RunPointsEarned { get; private set; }
        public int Wave { get; private set; }

        public event Action<bool> OnRunEnd;
        public event Action OnRunStart;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDiedEvent>(HandlePlayerDied);
            EventBus.Subscribe<EnemyKilledEvent>(HandleEnemyKilled);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDiedEvent>(HandlePlayerDied);
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleEnemyKilled);
        }

        public void StartRun()
        {
            if (IsRunStarted) return;
            EnsureManagers();
            RunTime = 0f;
            RunPointsEarned = 0;
            Wave = 0;
            IsRunStarted = true;
            Player.ResetForRun();
            Enemies.BeginSpawning();
            Skills.BeginRun();
            Towers.BeginRun();
            Inventory.BeginRun();
            Gems.BeginRun();
            EventBus.Publish(new RunStartedEvent());
            OnRunStart?.Invoke();
        }

        public void EndRun(bool victory)
        {
            if (!IsRunStarted) return;
            IsRunStarted = false;
            Enemies.StopSpawning();
            Enemies.ClearAll();
            Skills.EndRun();
            Towers.EndRun();
            Inventory.EndRun();
            Gems.EndRun();

            var data = GameManager.Instance.PlayerData;
            data.MetaPoints += RunPointsEarned;
            data.RunsPlayed += 1;
            if (victory) data.RunsWon += 1;
            SaveLoadManager.Save(data);

            EventBus.Publish(new RunEndedEvent(victory));
            OnRunEnd?.Invoke(victory);
        }

        private void Update()
        {
            if (!IsRunStarted) return;
            RunTime += Time.deltaTime;

            int targetWave = Mathf.FloorToInt(RunTime / 30f);
            if (targetWave > Wave)
            {
                Wave = targetWave;
                Enemies.SetDifficulty(Wave);
                EventBus.Publish(new WaveStartedEvent(Wave));
            }
        }

        private void EnsureManagers()
        {
            Player = SpawnIfMissing(Player, _playerManagerPrefab, nameof(PlayerManager));
            Enemies = SpawnIfMissing(Enemies, _enemiesManagerPrefab, nameof(EnemiesManager));
            Skills = SpawnIfMissing(Skills, _skillsManagerPrefab, nameof(SkillsManager));
            Towers = SpawnIfMissing(Towers, _towersManagerPrefab, nameof(TowersManager));
            Inventory = SpawnIfMissing(Inventory, _inventoryManagerPrefab, nameof(InventoryManager));
            Gems = SpawnIfMissing(Gems, _gemsManagerPrefab, nameof(GemsManager));
        }

        private T SpawnIfMissing<T>(T existing, T prefab, string fallbackName) where T : MonoBehaviour
        {
            if (existing != null) return existing;
            if (prefab != null) return Instantiate(prefab, transform);
            var go = new GameObject(fallbackName);
            go.transform.SetParent(transform, false);
            return go.AddComponent<T>();
        }

        private void HandlePlayerDied(PlayerDiedEvent _) => EndRun(false);

        private void HandleEnemyKilled(EnemyKilledEvent evt)
        {
            RunPointsEarned += 1;
            if (Player != null) Player.AddCurrency(CurrencyType.Gold, evt.GoldReward);
        }
    }
}
