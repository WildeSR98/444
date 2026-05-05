using System;
using System.Collections.Generic;
using UnityEngine;
using Vymesy.Enemies;
using Vymesy.Utils;

namespace Vymesy.Skills
{
    /// <summary>
    /// Tracks player run-XP awarded for kills and emits a <see cref="LevelUpEvent"/> when
    /// the threshold is crossed. Independent from the meta-tree progression.
    /// </summary>
    public class SkillProgressionManager : MonoBehaviour
    {
        [SerializeField] private int _baseXpToNext = 8;
        [SerializeField] private float _xpCurveExponent = 1.35f;
        [SerializeField] private float _xpCurveMultiplier = 1.6f;

        private readonly Dictionary<EnemyType, int> _xpByType = new Dictionary<EnemyType, int>
        {
            { EnemyType.Common,  1 },
            { EnemyType.Stalker, 2 },
            { EnemyType.Brute,   3 },
            { EnemyType.Wretch,  2 },
            { EnemyType.Elite,   8 },
            { EnemyType.Shiny,   5 },
            { EnemyType.Boss,   25 },
        };

        public int Level { get; private set; } = 1;
        public int CurrentXP { get; private set; }
        public int XPToNext { get; private set; }

        public event Action<int> OnLeveledUp;
        public event Action OnXPChanged;

        private void OnEnable()
        {
            EventBus.Subscribe<RunStartedEvent>(HandleRunStart);
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RunStartedEvent>(HandleRunStart);
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnd);
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleKilled);
        }

        private void HandleRunStart(RunStartedEvent _)
        {
            Level = 1;
            CurrentXP = 0;
            XPToNext = _baseXpToNext;
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleKilled);
            EventBus.Subscribe<EnemyKilledEvent>(HandleKilled);
            OnXPChanged?.Invoke();
        }

        private void HandleRunEnd(RunEndedEvent _)
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleKilled);
        }

        public void AwardXP(int amount)
        {
            if (amount <= 0) return;
            CurrentXP += amount;
            while (CurrentXP >= XPToNext)
            {
                CurrentXP -= XPToNext;
                Level++;
                XPToNext = Mathf.CeilToInt(_baseXpToNext * Mathf.Pow(Level, _xpCurveExponent) * _xpCurveMultiplier);
                OnLeveledUp?.Invoke(Level);
                EventBus.Publish(new LevelUpEvent(Level));
            }
            OnXPChanged?.Invoke();
        }

        private void HandleKilled(EnemyKilledEvent evt)
        {
            int xp = _xpByType.TryGetValue(evt.Type, out var v) ? v : 1;
            AwardXP(xp);
        }
    }

    public readonly struct LevelUpEvent
    {
        public readonly int NewLevel;
        public LevelUpEvent(int l) { NewLevel = l; }
    }
}
