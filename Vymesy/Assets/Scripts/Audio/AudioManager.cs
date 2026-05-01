using System.Collections.Generic;
using UnityEngine;
using Vymesy.Enemies;
using Vymesy.Skills;
using Vymesy.Utils;

namespace Vymesy.Audio
{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _musicSource;

        [Header("SFX Library")]
        [SerializeField] private AudioClip _enemyDeath;
        [SerializeField] private AudioClip _bossDeath;
        [SerializeField] private AudioClip _playerHit;
        [SerializeField] private AudioClip _runEnd;
        [SerializeField] private AudioClip _runStart;
        [SerializeField] private AudioClip _levelUp;
        [SerializeField] private AudioClip _pickup;

        [Header("Music")]
        [SerializeField] private AudioClip _menuMusic;
        [SerializeField] private AudioClip _runMusic;

        protected override void OnAwake()
        {
            base.OnAwake();
            if (_sfxSource == null) _sfxSource = gameObject.AddComponent<AudioSource>();
            if (_musicSource == null)
            {
                _musicSource = gameObject.AddComponent<AudioSource>();
                _musicSource.loop = true;
            }
            EnsureProceduralLibrary();
            EventBus.Subscribe<EnemyKilledEvent>(HandleEnemyKilled);
            EventBus.Subscribe<PlayerDamagedEvent>(HandlePlayerHit);
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnded);
            EventBus.Subscribe<RunStartedEvent>(HandleRunStarted);
            EventBus.Subscribe<LevelUpEvent>(HandleLevelUp);
            EventBus.Subscribe<ItemPickedUpEvent>(HandleItemPickup);
        }

        protected override void OnDestroy()
        {
            EventBus.Unsubscribe<EnemyKilledEvent>(HandleEnemyKilled);
            EventBus.Unsubscribe<PlayerDamagedEvent>(HandlePlayerHit);
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnded);
            EventBus.Unsubscribe<RunStartedEvent>(HandleRunStarted);
            EventBus.Unsubscribe<LevelUpEvent>(HandleLevelUp);
            EventBus.Unsubscribe<ItemPickedUpEvent>(HandleItemPickup);
            base.OnDestroy();
        }

        /// <summary>
        /// Fills any unassigned <see cref="AudioClip"/> slots with procedurally generated SFX
        /// so the demo plays sound even without bundled audio assets.
        /// </summary>
        private void EnsureProceduralLibrary()
        {
            if (_enemyDeath == null) _enemyDeath = ProceduralAudio.EnemyDeath();
            if (_bossDeath == null) _bossDeath = ProceduralAudio.BossDeath();
            if (_playerHit == null) _playerHit = ProceduralAudio.PlayerHit();
            if (_runStart == null) _runStart = ProceduralAudio.RunStart();
            if (_runEnd == null) _runEnd = ProceduralAudio.RunEnd();
            if (_levelUp == null) _levelUp = ProceduralAudio.LevelUp();
            if (_pickup == null) _pickup = ProceduralAudio.Pickup();
            if (_menuMusic == null) _menuMusic = ProceduralAudio.MenuPad();
            if (_runMusic == null) _runMusic = ProceduralAudio.RunPad();
        }

        public void PlayMenuMusic() => SetMusic(_menuMusic);
        public void PlayRunMusic() => SetMusic(_runMusic);

        public void Play(AudioClip clip, float volume = 1f)
        {
            if (clip == null || _sfxSource == null) return;
            _sfxSource.PlayOneShot(clip, volume);
        }

        private void SetMusic(AudioClip clip)
        {
            if (_musicSource == null) return;
            if (_musicSource.clip == clip && _musicSource.isPlaying) return;
            _musicSource.clip = clip;
            if (clip != null) _musicSource.Play(); else _musicSource.Stop();
        }

        private void HandleEnemyKilled(EnemyKilledEvent evt)
        {
            Play(evt.Type == EnemyType.Boss ? _bossDeath : _enemyDeath);
        }
        private void HandlePlayerHit(PlayerDamagedEvent _) => Play(_playerHit);
        private void HandleRunEnded(RunEndedEvent _) { Play(_runEnd); PlayMenuMusic(); }
        private void HandleRunStarted(RunStartedEvent _) { Play(_runStart); PlayRunMusic(); }
        private void HandleLevelUp(LevelUpEvent _) => Play(_levelUp);
        private void HandleItemPickup(ItemPickedUpEvent _) => Play(_pickup);
    }
}
