using System.Collections.Generic;
using UnityEngine;
using Vymesy.Achievements;
using Vymesy.Enemies;
using Vymesy.Enemies.AI;
using Vymesy.Gems;
using Vymesy.Inventory;
using Vymesy.MetaTree;
using Vymesy.Player;
using Vymesy.Projectiles;
using Vymesy.Skills;
using Vymesy.Towers;
using Vymesy.VFX;

namespace Vymesy.Demo
{
    /// <summary>
    /// Procedurally builds runtime ScriptableObject instances and prefab GameObjects so the demo
    /// can run without any imported assets. None of these are persisted to disk.
    /// </summary>
    public class DemoContent
    {
        public class EnemyEntry
        {
            public EnemyDefinition Definition;
            public GameObject Prefab;
            public int Weight;
            public int MinWave;
        }

        public class TowerCatalogEntry
        {
            public TowerDefinition Definition;
            public int Weight;
        }

        // ---------- Projectile prefabs ----------

        public GameObject MakeProjectilePrefab(string poolKey, Color color)
        {
            var go = NewProjectileSkeleton(poolKey, color, DemoSprites.Shape.Square, 16);
            var p = go.AddComponent<Projectile>();
            p.Pierce = 0;
            return go;
        }

        public GameObject MakeHomingProjectilePrefab(string poolKey, Color color)
        {
            var go = NewProjectileSkeleton(poolKey, color, DemoSprites.Shape.Diamond, 18);
            go.AddComponent<HomingProjectile>();
            return go;
        }

        public GameObject MakeOrbitProjectilePrefab(string poolKey, Color color)
        {
            var go = NewProjectileSkeleton(poolKey, color, DemoSprites.Shape.Circle, 14);
            go.AddComponent<OrbitProjectile>();
            return go;
        }

        private static GameObject NewProjectileSkeleton(string name, Color color, DemoSprites.Shape shape, int size)
        {
            var go = new GameObject(name);
            go.SetActive(false);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = DemoSprites.Get(shape, color, size);
            sr.sortingOrder = 4;
            return go;
        }

        // ---------- Enemy roster ----------

        public List<EnemyEntry> BuildEnemyRoster()
        {
            var list = new List<EnemyEntry>();

            list.Add(BuildEnemy("Common Vymes", EnemyType.Common, hp: 18, speed: 2.0f, dmg: 6,
                tint: new Color(0.35f, 0.25f, 0.25f), shape: DemoSprites.Shape.Circle, gold: 1, weight: 60, minWave: 0));

            list.Add(BuildEnemy("Stalker Vymes", EnemyType.Stalker, hp: 12, speed: 3.4f, dmg: 5,
                tint: new Color(0.4f, 0.15f, 0.45f), shape: DemoSprites.Shape.Diamond, gold: 2, weight: 30, minWave: 1));

            list.Add(BuildEnemy("Brute Vymes", EnemyType.Brute, hp: 60, speed: 1.4f, dmg: 14,
                tint: new Color(0.45f, 0.1f, 0.1f), shape: DemoSprites.Shape.Square, gold: 3, weight: 18, minWave: 2));

            var wretch = BuildEnemy("Wretch Vymes", EnemyType.Wretch, hp: 25, speed: 1.6f, dmg: 4,
                tint: new Color(0.2f, 0.55f, 0.25f), shape: DemoSprites.Shape.Cross, gold: 3, weight: 14, minWave: 3);
            wretch.Definition.IsRanged = true;
            wretch.Definition.AttackRange = 6f;
            wretch.Definition.AttackInterval = 1.6f;
            wretch.Definition.ProjectileSpeed = 5f;
            wretch.Definition.ProjectilePoolKey = "proj_demo"; // re-use shared pool
            list.Add(wretch);

            var elite = BuildEnemy("Elite Vymes", EnemyType.Elite, hp: 200, speed: 1.8f, dmg: 18,
                tint: new Color(0.85f, 0.5f, 1f), shape: DemoSprites.Shape.Square, gold: 12, weight: 5, minWave: 5);
            elite.Definition.SoulShardDropChance = 0.5f;
            list.Add(elite);

            var boss = BuildEnemy("Boss Vymes", EnemyType.Boss, hp: 1200, speed: 1.0f, dmg: 30,
                tint: new Color(1f, 0.25f, 0.2f), shape: DemoSprites.Shape.Cross, gold: 80, weight: 1, minWave: 10);
            list.Add(boss);

            var shiny = BuildEnemy("Shiny Vymes", EnemyType.Shiny, hp: 35, speed: 4.5f, dmg: 6,
                tint: new Color(1f, 0.95f, 0.4f), shape: DemoSprites.Shape.Diamond, gold: 25, weight: 2, minWave: 1);
            shiny.Definition.ItemDropChance = 1f;
            list.Add(shiny);

            return list;
        }

        private EnemyEntry BuildEnemy(string name, EnemyType type, float hp, float speed, float dmg, Color tint, DemoSprites.Shape shape, int gold, int weight, int minWave)
        {
            var def = ScriptableObject.CreateInstance<EnemyDefinition>();
            def.name = name;
            def.Type = type;
            def.DisplayName = name;
            def.MaxHealth = hp;
            def.MoveSpeed = speed;
            def.ContactDamage = dmg;
            def.GoldDrop = gold;
            def.Tint = tint;
            def.Sprite = DemoSprites.Get(shape, tint, 32);
            def.ColliderSize = new Vector2(0.6f, 0.6f);

            var prefab = new GameObject(name);
            prefab.SetActive(false);
            var sr = prefab.AddComponent<SpriteRenderer>();
            sr.sprite = def.Sprite;
            sr.color = tint;
            sr.sortingOrder = 3;

            var rb = prefab.AddComponent<Rigidbody2D>();
            rb.gravityScale = 0f;
            rb.freezeRotation = true;
#if UNITY_6000_0_OR_NEWER
            rb.linearDamping = 4f;
#else
            rb.drag = 4f;
#endif

            var col = prefab.AddComponent<CircleCollider2D>();
            col.radius = 0.32f;

            prefab.AddComponent<EnemyHealth>();
            switch (type)
            {
                case EnemyType.Stalker: prefab.AddComponent<EnemyAIStalker>(); break;
                case EnemyType.Brute:   prefab.AddComponent<EnemyAIBrute>();   break;
                case EnemyType.Boss:    prefab.AddComponent<EnemyAIBoss>();    break;
                default:                prefab.AddComponent<EnemyAI>();        break;
            }
            prefab.AddComponent<EnemyView>();
            prefab.AddComponent<EnemyController>();
            // Death VFX hook (created in step 4 of the demo content).
            prefab.AddComponent<DamageFlash>();
            prefab.AddComponent<DeathBurst>();
            prefab.AddComponent<DamageNumberPopup>();
            // Bosses and elites get an outlined silhouette to stand out on screen.
            if (type == EnemyType.Boss || type == EnemyType.Elite || type == EnemyType.Shiny)
            {
                var outline = prefab.AddComponent<SpriteOutlineEffect>();
                outline.SetColor(type == EnemyType.Boss ? new Color(1f, 0.3f, 0.3f) :
                                 type == EnemyType.Elite ? new Color(0.95f, 0.6f, 1f) :
                                                            new Color(1f, 0.95f, 0.4f));
            }
            return new EnemyEntry { Definition = def, Prefab = prefab, Weight = weight, MinWave = minWave };
        }

        // ---------- Skills ----------

        public List<SkillBase> BuildStartingSkills()
        {
            var skills = new List<SkillBase>();

            var bolt = ScriptableObject.CreateInstance<ProjectileSkill>();
            bolt.name = "Светлая стрела";
            bolt.DisplayName = "Светлая стрела";
            bolt.Description = "Базовая авто-стрела по ближайшему вымесу.";
            bolt.Cooldown = 0.6f;
            bolt.BaseDamage = 8f;
            bolt.ProjectilePoolKey = "proj_demo";
            bolt.BaseProjectiles = 1;
            bolt.SpreadDegrees = 0f;
            bolt.ProjectileSpeed = 14f;
            bolt.Range = 9f;
            skills.Add(bolt);

            var nova = ScriptableObject.CreateInstance<AoESkill>();
            nova.name = "Святая нова";
            nova.DisplayName = "Святая нова";
            nova.Description = "Раз в 4 секунды наносит урон вымесам в радиусе.";
            nova.Cooldown = 4f;
            nova.BaseDamage = 14f;
            nova.Radius = 3.5f;
            nova.KnockbackForce = 2f;
            skills.Add(nova);

            var chain = ScriptableObject.CreateInstance<ChainSkill>();
            chain.name = "Цепная молния";
            chain.DisplayName = "Цепная молния";
            chain.Description = "Молния прыгает между вымесами.";
            chain.Cooldown = 2.5f;
            chain.BaseDamage = 12f;
            chain.Bounces = 4;
            chain.JumpRange = 4f;
            skills.Add(chain);

            var homing = ScriptableObject.CreateInstance<HomingSkill>();
            homing.name = "Самонаводящие иглы";
            homing.DisplayName = "Самонаводящие иглы";
            homing.Description = "Тройка снарядов догоняет ближайших вымесов.";
            homing.Cooldown = 1.6f;
            homing.BaseDamage = 7f;
            homing.ProjectilePoolKey = "proj_homing";
            homing.Missiles = 3;
            homing.ProjectileSpeed = 8f;
            homing.Range = 12f;
            skills.Add(homing);

            var orbit = ScriptableObject.CreateInstance<OrbitSkill>();
            orbit.name = "Кольцо света";
            orbit.DisplayName = "Кольцо света";
            orbit.Description = "Три орба света кружат вокруг игрока.";
            orbit.Cooldown = 5f;
            orbit.BaseDamage = 6f;
            orbit.ProjectilePoolKey = "proj_orbit";
            orbit.OrbCount = 3;
            orbit.OrbitRadius = 1.6f;
            orbit.AngularSpeed = 200f;
            orbit.Lifetime = 4.5f;
            skills.Add(orbit);

            return skills;
        }

        // ---------- Towers ----------

        public List<TowerCatalogEntry> BuildTowerCatalog()
        {
            var list = new List<TowerCatalogEntry>();

            var aoe = ScriptableObject.CreateInstance<TowerDefinition>();
            aoe.name = "Алтарь возмездия";
            aoe.Type = TowerType.AoE;
            aoe.DisplayName = "Алтарь возмездия";
            aoe.TickInterval = 1.4f;
            aoe.Range = 4.5f;
            aoe.Damage = 10f;
            aoe.Prefab = MakeTowerPrefab("Алтарь возмездия", typeof(AoETower), new Color(1f, 0.5f, 0.4f));
            list.Add(new TowerCatalogEntry { Definition = aoe, Weight = 30 });

            var circle = ScriptableObject.CreateInstance<TowerDefinition>();
            circle.name = "Лучник Света";
            circle.Type = TowerType.Circle;
            circle.DisplayName = "Лучник Света";
            circle.TickInterval = 0.8f;
            circle.Range = 6f;
            circle.Damage = 7f;
            circle.Prefab = MakeTowerPrefab("Лучник Света", typeof(CircleTower), new Color(0.95f, 0.95f, 0.6f));
            list.Add(new TowerCatalogEntry { Definition = circle, Weight = 30 });

            var poison = ScriptableObject.CreateInstance<TowerDefinition>();
            poison.name = "Тлен";
            poison.Type = TowerType.Poison;
            poison.DisplayName = "Тлен";
            poison.TickInterval = 0.5f;
            poison.Range = 3.5f;
            poison.Damage = 3f;
            poison.Prefab = MakeTowerPrefab("Тлен", typeof(PoisonTower), new Color(0.4f, 0.85f, 0.3f));
            list.Add(new TowerCatalogEntry { Definition = poison, Weight = 20 });

            var lightning = ScriptableObject.CreateInstance<TowerDefinition>();
            lightning.name = "Шторм";
            lightning.Type = TowerType.Lightning;
            lightning.DisplayName = "Шторм";
            lightning.TickInterval = 1.2f;
            lightning.Range = 5f;
            lightning.Damage = 6f;
            lightning.Prefab = MakeTowerPrefab("Шторм", typeof(LightningTower), new Color(0.55f, 0.85f, 1f));
            list.Add(new TowerCatalogEntry { Definition = lightning, Weight = 18 });

            var aura = ScriptableObject.CreateInstance<TowerDefinition>();
            aura.name = "Священная аура";
            aura.Type = TowerType.Aura;
            aura.DisplayName = "Священная аура";
            aura.TickInterval = 0.4f;
            aura.Range = 2.8f;
            aura.Damage = 1.2f;
            aura.Prefab = MakeTowerPrefab("Священная аура", typeof(AuraTower), new Color(0.85f, 0.95f, 1f));
            list.Add(new TowerCatalogEntry { Definition = aura, Weight = 22 });

            var gold = ScriptableObject.CreateInstance<TowerDefinition>();
            gold.name = "Сундук";
            gold.Type = TowerType.Gold;
            gold.DisplayName = "Сундук";
            gold.TickInterval = 2f;
            gold.GoldPerTick = 1;
            gold.Prefab = MakeTowerPrefab("Сундук", typeof(GoldTower), new Color(1f, 0.85f, 0.2f));
            list.Add(new TowerCatalogEntry { Definition = gold, Weight = 10 });

            return list;
        }

        private GameObject MakeTowerPrefab(string name, System.Type towerType, Color tint)
        {
            var go = new GameObject(name);
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = DemoSprites.Get(DemoSprites.Shape.Square, tint, 28);
            sr.sortingOrder = 2;
            go.AddComponent(towerType);
            return go;
        }

        // ---------- Items ----------

        public List<ItemData> BuildItemPool()
        {
            var pool = new List<ItemData>();
            pool.Add(BuildItem("Кулон Стойкости", ItemRarity.Normal, m => m.MaxHealth = 15));
            pool.Add(BuildItem("Перо Тени", ItemRarity.Normal, m => m.MoveSpeed = 0.4f));
            pool.Add(BuildItem("Кольцо Огня", ItemRarity.Rare, m => m.DamageMultiplier = 0.15f));
            pool.Add(BuildItem("Глаз Прицела", ItemRarity.Rare, m => m.CritChance = 0.05f));
            pool.Add(BuildItem("Ритм Боя", ItemRarity.Epic, m => m.AttackSpeedMultiplier = 0.20f));
            pool.Add(BuildItem("Трофей Вымеса", ItemRarity.Epic, m => { m.GoldMultiplier = 0.25f; m.PickupRadius = 0.5f; }));
            pool.Add(BuildItem("Слово Хранителя", ItemRarity.Legendary, m => { m.DamageMultiplier = 0.25f; m.HealthRegenPerSecond = 1f; }));
            return pool;
        }

        private ItemData BuildItem(string name, ItemRarity rarity, System.Action<PlayerStatsModifier> apply)
        {
            var item = ScriptableObject.CreateInstance<ItemData>();
            item.name = name;
            item.DisplayName = name;
            item.Rarity = rarity;
            item.SellGold = rarity switch
            {
                ItemRarity.Normal => 5,
                ItemRarity.Rare => 15,
                ItemRarity.Epic => 40,
                ItemRarity.Legendary => 120,
                _ => 5,
            };
            apply?.Invoke(item.Modifier);
            return item;
        }

        // ---------- Gems ----------

        public List<GemData> BuildGemCatalog()
        {
            var list = new List<GemData>();
            list.Add(BuildGem("Рубин Ярости", GemStat.Damage, 0.05f));
            list.Add(BuildGem("Сапфир Темпа", GemStat.AttackSpeed, 0.04f));
            list.Add(BuildGem("Изумруд Скрытности", GemStat.MoveSpeed, 0.10f));
            list.Add(BuildGem("Топаз Точности", GemStat.CritChance, 0.01f));
            list.Add(BuildGem("Жемчуг Жизни", GemStat.MaxHealth, 0.05f));
            list.Add(BuildGem("Алмаз Удачи", GemStat.GoldFind, 0.05f));
            return list;
        }

        private GemData BuildGem(string name, GemStat stat, float perLevel)
        {
            var gem = ScriptableObject.CreateInstance<GemData>();
            gem.name = name;
            gem.DisplayName = name;
            gem.Stat = stat;
            gem.ValuePerLevel = perLevel;
            gem.MaxLevel = 20;
            return gem;
        }

        // ---------- Tree + achievements ----------

        public List<TreeNode> BuildTreeNodes()
        {
            var list = new List<TreeNode>();
            list.Add(BuildNode("tree_hp", "+25 HP", 1, m => m.MaxHealth = 25));
            list.Add(BuildNode("tree_dmg", "+10% урон", 2, m => m.DamageMultiplier = 0.10f));
            list.Add(BuildNode("tree_speed", "+10% скорость", 1, m => m.MoveSpeed = 0.5f));
            list.Add(BuildNode("tree_pickup", "+0.5 радиус подбора", 1, m => m.PickupRadius = 0.5f));
            list.Add(BuildNode("tree_gold", "+25% золото", 2, m => m.GoldMultiplier = 0.25f));
            return list;
        }

        private TreeNode BuildNode(string id, string display, int cost, System.Action<PlayerStatsModifier> apply)
        {
            var node = ScriptableObject.CreateInstance<TreeNode>();
            node.name = id;
            node.Id = id;
            node.DisplayName = display;
            node.Cost = cost;
            apply?.Invoke(node.Modifier);
            return node;
        }

        public List<Achievement> BuildAchievements()
        {
            var list = new List<Achievement>();
            list.Add(BuildAch("ach_first_blood", "Первая кровь", AchievementType.EnemiesKilled, 1, 1));
            list.Add(BuildAch("ach_50_kills", "Истребитель", AchievementType.EnemiesKilled, 50, 2));
            list.Add(BuildAch("ach_500_kills", "Жнец", AchievementType.EnemiesKilled, 500, 5));
            list.Add(BuildAch("ach_wave_5", "Закалённый", AchievementType.ReachWave, 5, 3));
            list.Add(BuildAch("ach_gold_500", "Скряга", AchievementType.CollectGold, 500, 2));
            return list;
        }

        private Achievement BuildAch(string id, string name, AchievementType type, int threshold, int reward)
        {
            var a = ScriptableObject.CreateInstance<Achievement>();
            a.name = id;
            a.Id = id;
            a.DisplayName = name;
            a.Type = type;
            a.Threshold = threshold;
            a.MetaPointsReward = reward;
            return a;
        }
    }
}
