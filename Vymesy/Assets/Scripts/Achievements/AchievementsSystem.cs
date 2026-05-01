using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Enemies;
using Vymesy.Player;
using Vymesy.Save;
using Vymesy.Utils;

namespace Vymesy.Achievements
{
    public class AchievementsSystem : MonoBehaviour
    {
        [SerializeField] private List<Achievement> _achievements = new List<Achievement>();

        private readonly Dictionary<AchievementType, int> _counters = new Dictionary<AchievementType, int>();

        private void OnEnable()
        {
            EventBus.Subscribe<EnemyKilledEvent>(HandleEnemyKilled);
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnded);
            EventBus.Subscribe<CurrencyChangedEvent>(HandleCurrencyChanged);
            EventBus.Subscribe<WaveStartedEvent>(HandleWaveStarted);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleEnemyKilled);
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnded);
            EventBus.Unsubscribe<CurrencyChangedEvent>(HandleCurrencyChanged);
            EventBus.Unsubscribe<WaveStartedEvent>(HandleWaveStarted);
        }

        private void HandleEnemyKilled(EnemyKilledEvent _) => Bump(AchievementType.EnemiesKilled, 1);
        private void HandleWaveStarted(WaveStartedEvent evt) => Set(AchievementType.ReachWave, evt.Wave);
        private void HandleRunEnded(RunEndedEvent evt) { if (evt.Victory) Bump(AchievementType.RunsWon, 1); }
        private void HandleCurrencyChanged(CurrencyChangedEvent evt)
        {
            if (evt.Type == CurrencyType.Gold) Set(AchievementType.CollectGold, evt.NewAmount);
        }

        private void Bump(AchievementType type, int amount)
        {
            int v = _counters.TryGetValue(type, out var c) ? c : 0;
            _counters[type] = v + amount;
            CheckUnlocks(type);
        }

        private void Set(AchievementType type, int value)
        {
            int v = _counters.TryGetValue(type, out var c) ? c : 0;
            if (value > v) _counters[type] = value;
            CheckUnlocks(type);
        }

        private void CheckUnlocks(AchievementType type)
        {
            if (!GameManager.HasInstance) return;
            var data = GameManager.Instance.PlayerData;
            int progress = _counters[type];
            for (int i = 0; i < _achievements.Count; i++)
            {
                var a = _achievements[i];
                if (a == null || a.Type != type) continue;
                if (data.UnlockedAchievements.Contains(a.Id)) continue;
                if (progress >= a.Threshold)
                {
                    data.UnlockedAchievements.Add(a.Id);
                    data.MetaPoints += a.MetaPointsReward;
                    SaveLoadManager.Save(data);
                    Debug.Log($"[Achievement] Unlocked '{a.DisplayName}' (+{a.MetaPointsReward} MetaPoints)");
                }
            }
        }
    }
}
