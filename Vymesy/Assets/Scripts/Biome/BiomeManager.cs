using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Utils;

namespace Vymesy.Biome
{
    /// <summary>
    /// Cycles biomes every <see cref="WavesPerBiome"/> waves. Each cycle re-tints the camera
    /// background and re-spawns ground hazards in a ring around the player.
    /// </summary>
    public class BiomeManager : MonoBehaviour
    {
        [SerializeField] private int _wavesPerBiome = 3;
        [SerializeField] private float _hazardRingMin = 3f;
        [SerializeField] private float _hazardRingMax = 7f;
        [SerializeField] private List<Biome> _biomes = new List<Biome>();

        public int WavesPerBiome => _wavesPerBiome;
        public IReadOnlyList<Biome> Biomes => _biomes;
        public Biome CurrentBiome { get; private set; }

        private readonly List<Hazard> _activeHazards = new List<Hazard>();

        private void Awake()
        {
            if (_biomes.Count == 0) BuildDefaultBiomes();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<RunStartedEvent>(HandleRunStart);
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnd);
            EventBus.Subscribe<WaveStartedEvent>(HandleWaveStart);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RunStartedEvent>(HandleRunStart);
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnd);
            EventBus.Unsubscribe<WaveStartedEvent>(HandleWaveStart);
        }

        private void HandleRunStart(RunStartedEvent _) => SwitchBiome(0);
        private void HandleRunEnd(RunEndedEvent _) => ClearHazards();

        private void HandleWaveStart(WaveStartedEvent evt)
        {
            int biomeIndex = Mathf.Max(0, evt.Wave) / Mathf.Max(1, _wavesPerBiome);
            biomeIndex %= _biomes.Count;
            if (CurrentBiome != _biomes[biomeIndex]) SwitchBiome(biomeIndex);
        }

        private void SwitchBiome(int index)
        {
            if (_biomes.Count == 0) return;
            CurrentBiome = _biomes[Mathf.Clamp(index, 0, _biomes.Count - 1)];
            var cam = Camera.main;
            if (cam != null) cam.backgroundColor = CurrentBiome.BackgroundColor;
            ClearHazards();
            SpawnHazards();
        }

        private void SpawnHazards()
        {
            if (CurrentBiome == null || CurrentBiome.HazardKind == BiomeHazard.None) return;
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            var player = rm != null ? rm.Player : null;
            Vector3 origin = player != null ? player.transform.position : Vector3.zero;
            for (int i = 0; i < CurrentBiome.HazardCount; i++)
            {
                Vector2 offset = MathUtils.RandomInAnnulus(_hazardRingMin, _hazardRingMax);
                Vector3 pos = origin + (Vector3)offset;
                var hazard = Hazard.Spawn(CurrentBiome.HazardKind, pos, CurrentBiome.HazardRadius, CurrentBiome.HazardColor, CurrentBiome.HazardDamage);
                _activeHazards.Add(hazard);
            }
        }

        private void ClearHazards()
        {
            for (int i = 0; i < _activeHazards.Count; i++)
            {
                if (_activeHazards[i] != null) Destroy(_activeHazards[i].gameObject);
            }
            _activeHazards.Clear();
        }

        private void BuildDefaultBiomes()
        {
            _biomes.Add(new Biome
            {
                Id = "twilight_marsh",
                DisplayName = "Сумеречная топь",
                BackgroundColor = new Color(0.05f, 0.07f, 0.06f),
                FogTint = new Color(0.4f, 0.5f, 0.4f, 0.5f),
                HazardColor = new Color(0.55f, 0.85f, 0.4f, 0.7f),
                HazardKind = BiomeHazard.Fog,
                HazardCount = 5,
                HazardRadius = 1.6f,
                HazardDamage = 0f,
            });
            _biomes.Add(new Biome
            {
                Id = "ashen_wastes",
                DisplayName = "Пепельные пустоши",
                BackgroundColor = new Color(0.12f, 0.05f, 0.04f),
                FogTint = new Color(0.7f, 0.3f, 0.2f, 0.4f),
                HazardColor = new Color(1f, 0.4f, 0.2f, 0.85f),
                HazardKind = BiomeHazard.Lava,
                HazardCount = 4,
                HazardRadius = 1.3f,
                HazardDamage = 5f,
            });
            _biomes.Add(new Biome
            {
                Id = "frozen_crypt",
                DisplayName = "Промёрзлый склеп",
                BackgroundColor = new Color(0.05f, 0.07f, 0.12f),
                FogTint = new Color(0.5f, 0.7f, 0.95f, 0.5f),
                HazardColor = new Color(0.7f, 0.85f, 1f, 0.85f),
                HazardKind = BiomeHazard.Ice,
                HazardCount = 4,
                HazardRadius = 1.5f,
                HazardDamage = 0f,
                EnemySpeedMultiplier = 0.85f,
            });
            _biomes.Add(new Biome
            {
                Id = "bloodmoon_hold",
                DisplayName = "Кровавая твердь",
                BackgroundColor = new Color(0.16f, 0.04f, 0.06f),
                FogTint = new Color(0.6f, 0.1f, 0.15f, 0.4f),
                HazardColor = new Color(0.85f, 0.15f, 0.25f, 0.9f),
                HazardKind = BiomeHazard.ShadowVent,
                HazardCount = 5,
                HazardRadius = 1.0f,
                HazardDamage = 3f,
                EnemyHealthMultiplier = 1.25f,
            });
        }
    }
}
