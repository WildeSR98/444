using System.Collections.Generic;
using UnityEngine;
using Vymesy.Achievements;
using Vymesy.Audio;
using Vymesy.Core;
using Vymesy.Enemies;
using Vymesy.Gems;
using Vymesy.Inventory;
using Vymesy.MetaTree;
using Vymesy.Pooling;
using Vymesy.Player;
using Vymesy.Projectiles;
using Vymesy.Biome;
using Vymesy.Localization;
using Vymesy.Save;
using Vymesy.Skills;
using Vymesy.Towers;
using Vymesy.UI;
using Vymesy.VFX;

namespace Vymesy.Demo
{
    /// <summary>
    /// Builds a fully runnable demo scene programmatically. Drop this single component
    /// onto an empty GameObject in any scene and press Play — it constructs the player,
    /// camera rig, all managers, and procedural content (enemies, skills, items, gems,
    /// towers, achievements, meta nodes) without requiring any prefabs in the inspector.
    /// </summary>
    [DefaultExecutionOrder(-100)]
    public class DemoBootstrap : MonoBehaviour
    {
        [Header("Demo Settings")]
        [SerializeField] private bool _autoStartRun = true;
        [SerializeField] private float _spawnInterval = 0.7f;
        [SerializeField] private int _maxAlive = 80;
        [SerializeField] private float _spawnRingMin = 8f;
        [SerializeField] private float _spawnRingMax = 12f;

        private GameManager _game;
        private PlayerManager _player;
        private CameraFollow _cameraFollow;

        private readonly DemoContent _content = new DemoContent();

        private void Awake()
        {
            EnsureBackground();
            _game = GameManager.Instance; // creates instance if missing
            LocalizationManager.SetLanguage(_game.PlayerData != null ? _game.PlayerData.Language : LocalizationManager.DefaultLanguage);
            _player = BuildPlayer();
            BuildCameraRig(_player.transform);

            var rm = _game.RunManager;
            BuildAllManagers(rm);
            BuildContent(rm);
            BuildHud();

            if (_autoStartRun) rm.StartRun();
        }

        private static void EnsureBackground()
        {
            Camera.main?.gameObject.SetActive(true);
            if (Camera.main != null) Camera.main.backgroundColor = new Color(0.05f, 0.04f, 0.07f);
        }

        private PlayerManager BuildPlayer()
        {
            var go = new GameObject("Player");
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = DemoSprites.Get(DemoSprites.Shape.Circle, new Color(0.95f, 0.92f, 0.7f));
            sr.sortingOrder = 5;

            var rb = go.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
#if UNITY_6000_0_OR_NEWER
            rb.linearDamping = 6f;
#else
            rb.drag = 6f;
#endif

            var col = go.AddComponent<CircleCollider2D>();
            col.radius = 0.4f;

            go.AddComponent<PlayerController>();
            go.AddComponent<PlayerHealth>();
            var pm = go.AddComponent<PlayerManager>();
            return pm;
        }

        private void BuildCameraRig(Transform target)
        {
            var rig = new GameObject("CameraRig");
            var follow = rig.AddComponent<CameraFollow>();
            follow.SetTarget(target);
            _cameraFollow = follow;

            // Reuse the existing main camera when present, otherwise build one.
            Camera cam = Camera.main;
            if (cam == null)
            {
                var camGo = new GameObject("Main Camera");
                camGo.tag = "MainCamera";
                cam = camGo.AddComponent<Camera>();
                camGo.AddComponent<AudioListener>();
            }
            cam.orthographic = true;
            cam.orthographicSize = 7f;
            cam.transform.SetParent(rig.transform, worldPositionStays: false);
            cam.transform.localPosition = new Vector3(0f, 0f, -10f);
            cam.gameObject.AddComponent<CameraShake>();
            cam.gameObject.AddComponent<DarkVignetteOverlay>();
        }

        private void BuildAllManagers(RunManager rm)
        {
            rm.gameObject.AddComponent<LootDropper>();

            // SkillsManager / ProjectilesManager / Towers / Inventory / Gems are already
            // created lazily inside RunManager.EnsureManagers via SpawnIfMissing,
            // but they are added on RunManager.StartRun(). To configure them up-front
            // we create them now so we can register content before the run begins.
            EnsureChild<SkillsManager>(rm);
            EnsureChild<TowersManager>(rm);
            EnsureChild<InventoryManager>(rm);
            EnsureChild<GemsManager>(rm);
            EnsureChild<EnemiesManager>(rm);
            EnsureChild<TreeManager>(rm);
            EnsureChild<AchievementsSystem>(rm);

            // Phase 2 systems.
            EnsureChild<SkillProgressionManager>(rm);
            EnsureChild<WaveDirector>(rm);
            EnsureChild<BiomeManager>(rm);

            // Ensure singletons exist.
            _ = ProjectilesManager.Instance;
            _ = AudioManager.Instance;
        }

        private static T EnsureChild<T>(Component parent) where T : MonoBehaviour
        {
            var existing = parent.GetComponentInChildren<T>(true);
            if (existing != null) return existing;
            var go = new GameObject(typeof(T).Name);
            go.transform.SetParent(parent.transform, false);
            return go.AddComponent<T>();
        }

        private void BuildContent(RunManager rm)
        {
            var enemies = rm.GetComponentInChildren<EnemiesManager>(true);
            var skills = rm.GetComponentInChildren<SkillsManager>(true);
            var towers = rm.GetComponentInChildren<TowersManager>(true);
            var gems = rm.GetComponentInChildren<GemsManager>(true);
            var loot = rm.GetComponent<LootDropper>();
            var tree = rm.GetComponentInChildren<TreeManager>(true);
            var ach = rm.GetComponentInChildren<AchievementsSystem>(true);

            // Player needs to be the spawn target.
            enemies.SetTarget(_player.transform);
            enemies.ConfigureSpawn(_spawnInterval, _spawnInterval * 0.25f, _spawnRingMin, _spawnRingMax, _maxAlive);

            // Register projectile prefabs in the projectiles pool.
            ProjectilesManager.Instance.Register("proj_demo", _content.MakeProjectilePrefab("proj_demo", new Color(1f, 0.85f, 0.3f)), prewarm: 32);
            ProjectilesManager.Instance.Register("proj_chain", _content.MakeProjectilePrefab("proj_chain", new Color(0.5f, 0.85f, 1f)), prewarm: 16);
            ProjectilesManager.Instance.Register("proj_orbit", _content.MakeOrbitProjectilePrefab("proj_orbit", new Color(0.85f, 0.6f, 1f)), prewarm: 8);
            ProjectilesManager.Instance.Register("proj_homing", _content.MakeHomingProjectilePrefab("proj_homing", new Color(1f, 0.4f, 0.4f)), prewarm: 16);

            // Enemy roster.
            foreach (var e in _content.BuildEnemyRoster())
                enemies.AddEntry(e.Definition, e.Prefab, e.Weight, e.MinWave, prewarm: 8);

            // Demo skill loadout.
            foreach (var s in _content.BuildStartingSkills()) skills.Equip(s);

            // Tower catalog (auto-spawn one Circle tower at origin so the player has help).
            foreach (var t in _content.BuildTowerCatalog()) towers.AddCatalogEntry(t.Definition, t.Weight);
            var firstTower = _content.BuildTowerCatalog()[0].Definition;
            towers.SpawnTower(firstTower, Vector3.zero);

            // Items + gems pools and starting gem.
            loot?.SetItemPool(_content.BuildItemPool());
            loot?.SetBaseItemChance(0.07f);

            foreach (var g in _content.BuildGemCatalog()) gems.AddCatalogEntry(g);
            gems.Equip(0, _content.BuildGemCatalog()[0], level: 3);

            // Meta tree + achievements.
            foreach (var n in _content.BuildTreeNodes()) tree.AddNode(n);
            foreach (var a in _content.BuildAchievements()) ach.AddAchievement(a);
        }

        private void BuildHud()
        {
            var hudGo = new GameObject("DemoHUD");
            hudGo.AddComponent<DemoHUD>();

            // Phase 2 IMGUI overlays.
            var levelUp = hudGo.AddComponent<LevelUpModal>();
            levelUp.SetUnlockPool(_content.BuildUnlockableSkills());
            hudGo.AddComponent<AltarShop>();
            hudGo.AddComponent<StatsScreen>();
            hudGo.AddComponent<TouchJoystick>();

            // Wire the joystick into the player so mobile builds receive movement input.
            var ctrl = _player != null ? _player.GetComponent<PlayerController>() : null;
            var joystick = hudGo.GetComponent<TouchJoystick>();
            if (ctrl != null && joystick != null) ctrl.TouchInput = joystick;
        }
    }
}
