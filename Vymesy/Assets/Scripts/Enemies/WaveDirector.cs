using System.Collections;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Utils;
using Vymesy.VFX;

namespace Vymesy.Enemies
{
    /// <summary>
    /// Listens to <see cref="WaveStartedEvent"/> and stages special encounters:
    /// <list type="bullet">
    /// <item>Every <see cref="HeraldEvery"/> waves — spawns a small pack of elites with a banner.</item>
    /// <item>Every <see cref="BossEvery"/> waves (and Wave > 0) — summons a boss with a telegraphed shockwave.</item>
    /// </list>
    /// </summary>
    public class WaveDirector : MonoBehaviour
    {
        [SerializeField] private int _heraldEvery = 5;
        [SerializeField] private int _bossEvery = 10;
        [SerializeField] private int _heraldEliteCount = 3;
        [SerializeField] private float _bossShockwaveTelegraph = 1.4f;
        [SerializeField] private float _bossShockwaveRadius = 4f;
        [SerializeField] private float _bossShockwaveDamage = 22f;

        public int HeraldEvery => _heraldEvery;
        public int BossEvery => _bossEvery;

        private EnemiesManager _enemies;

        private void OnEnable()
        {
            EventBus.Subscribe<WaveStartedEvent>(HandleWaveStarted);
            EventBus.Subscribe<RunStartedEvent>(HandleRunStart);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<WaveStartedEvent>(HandleWaveStarted);
            EventBus.Unsubscribe<RunStartedEvent>(HandleRunStart);
        }

        private void HandleRunStart(RunStartedEvent _)
        {
            _enemies = GameManager.HasInstance ? GameManager.Instance.RunManager?.Enemies : null;
        }

        private void HandleWaveStarted(WaveStartedEvent evt)
        {
            int wave = evt.Wave;
            if (wave <= 0) return;
            if (_enemies == null) _enemies = GameManager.HasInstance ? GameManager.Instance.RunManager?.Enemies : null;
            if (_enemies == null) return;

            if (_bossEvery > 0 && wave % _bossEvery == 0)
            {
                StartCoroutine(SummonBoss());
                return;
            }
            if (_heraldEvery > 0 && wave % _heraldEvery == 0)
            {
                StartCoroutine(SummonHeralds());
            }
        }

        private IEnumerator SummonHeralds()
        {
            for (int i = 0; i < _heraldEliteCount; i++)
            {
                _enemies.SpawnSpecific(EnemyType.Elite);
                yield return new WaitForSeconds(0.3f);
            }
        }

        private IEnumerator SummonBoss()
        {
            var boss = _enemies.SpawnSpecific(EnemyType.Boss);
            // Always plant a telegraphed shockwave at the player's position to give the
            // moment some weight even if the boss couldn't spawn.
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            Vector3 origin = rm != null && rm.Player != null ? rm.Player.transform.position : Vector3.zero;
            AoEWarning.Spawn(origin, _bossShockwaveRadius, _bossShockwaveTelegraph, _bossShockwaveDamage);
            yield break;
        }
    }
}
