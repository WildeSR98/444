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
            EnemiesKilledThisRun = 0;
            Wave = 0;
            IsRunStarted = true;
            Player.ResetForRun();
            var data = GameManager.Instance.PlayerData;
            if (data != null) Enemies.SetAscensionMultiplier(1f + 0.20f * data.AscensionLevel);
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
            int goldEarned = 0;
            int shardsEarned = 0;
            if (Player != null)
            {
                goldEarned = Player.Currency.Get(CurrencyType.Gold);
                shardsEarned = Player.Currency.Get(CurrencyType.SoulShards);
                data.Gold += goldEarned;
                data.SoulShards += shardsEarned;
            }
            if (Wave > data.HighestWave) data.HighestWave = Wave;
            if (victory && data.AscensionLevel > data.HighestAscensionCleared) data.HighestAscensionCleared = data.AscensionLevel;
            data.RunHistory.Add(new RunHistoryEntry
            {
                EndTimeUnix = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Victory = victory,
                WaveReached = Wave,
                EnemiesKilled = EnemiesKilledThisRun,
                GoldCollected = goldEarned,
                DurationSeconds = RunTime,
                AscensionLevel = data.AscensionLevel,
            });
            // Keep history bounded.
            const int MaxHistory = 100;
            if (data.RunHistory.Count > MaxHistory)
                data.RunHistory.RemoveRange(0, data.RunHistory.Count - MaxHistory);
            SaveLoadManager.Save(data);

            EventBus.Publish(new RunEndedEvent(victory));
            OnRunEnd?.Invoke(victory);
        }

        public int EnemiesKilledThisRun { get; private set; }

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
            // Reuse existing components in our hierarchy or anywhere in the scene before
            // falling back to instantiation. Lets VymesyDemoBootstrap pre-create the managers.
            Player = Player != null ? Player : FindAnywhere<PlayerManager>();
            Enemies = Enemies != null ? Enemies : FindAnywhere<EnemiesManager>();
            Skills = Skills != null ? Skills : FindAnywhere<SkillsManager>();
            Towers = Towers != null ? Towers : FindAnywhere<TowersManager>();
            Inventory = Inventory != null ? Inventory : FindAnywhere<InventoryManager>();
            Gems = Gems != null ? Gems : FindAnywhere<GemsManager>();

            Player = SpawnIfMissing(Player, _playerManagerPrefab, nameof(PlayerManager));
            Enemies = SpawnIfMissing(Enemies, _enemiesManagerPrefab, nameof(EnemiesManager));
            Skills = SpawnIfMissing(Skills, _skillsManagerPrefab, nameof(SkillsManager));
            Towers = SpawnIfMissing(Towers, _towersManagerPrefab, nameof(TowersManager));
            Inventory = SpawnIfMissing(Inventory, _inventoryManagerPrefab, nameof(InventoryManager));
            Gems = SpawnIfMissing(Gems, _gemsManagerPrefab, nameof(GemsManager));
        }

        private static T FindAnywhere<T>() where T : MonoBehaviour
        {
#if UNITY_2023_1_OR_NEWER
            return UnityEngine.Object.FindFirstObjectByType<T>(FindObjectsInactive.Include);
#else
            return UnityEngine.Object.FindObjectOfType<T>();
#endif
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
            EnemiesKilledThisRun += 1;
            if (Player != null) Player.AddCurrency(CurrencyType.Gold, evt.GoldReward);
        }
    }
}
