#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;
using Vymesy.Enemies;
using Vymesy.Skills;
using Vymesy.Towers;

namespace Vymesy.EditorTools
{
    /// <summary>
    /// "Vymesy Content Editor" — a single editor window that batches the most common
    /// content-authoring tasks (creating EnemyDefinition / Skill / Tower / Item / Gem
    /// ScriptableObjects with sane defaults) so they don't require navigating
    /// Project → Create… every time.
    /// </summary>
    public class ContentEditorWindow : EditorWindow
    {
        private const string ContentRoot = "Assets/Resources/Vymesy/Content";

        private string _enemyId = "vymes_new";
        private string _enemyName = "Новый вымес";
        private EnemyType _enemyType = EnemyType.Common;
        private float _enemyHp = 30f;
        private float _enemyDamage = 8f;
        private float _enemySpeed = 2.5f;

        private string _skillId = "skill_new";
        private string _skillName = "Новый скилл";
        private SkillKind _skillKind = SkillKind.Projectile;
        private float _skillCooldown = 1f;
        private float _skillDamage = 8f;

        private string _towerId = "tower_new";
        private string _towerName = "Новая башня";
        private TowerType _towerType = TowerType.AoE;

        private Vector2 _scroll;

        [MenuItem("Vymesy/Content Editor")]
        public static void Open()
        {
            GetWindow<ContentEditorWindow>("Vymesy Content");
        }

        private void OnGUI()
        {
            _scroll = GUILayout.BeginScrollView(_scroll);
            EditorGUILayout.LabelField("Vymesy Content Editor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                $"Создаёт ScriptableObject в \"{ContentRoot}\" с разумными значениями по умолчанию.\n" +
                "После создания актив выделяется в Project — отредактируй детали в инспекторе.",
                MessageType.Info);

            DrawEnemySection();
            EditorGUILayout.Space(8);
            DrawSkillSection();
            EditorGUILayout.Space(8);
            DrawTowerSection();
            EditorGUILayout.Space(8);
            DrawUtilitySection();
            GUILayout.EndScrollView();
        }

        private void DrawEnemySection()
        {
            EditorGUILayout.LabelField("Враг (Вымес)", EditorStyles.boldLabel);
            _enemyId = EditorGUILayout.TextField("Id", _enemyId);
            _enemyName = EditorGUILayout.TextField("Имя", _enemyName);
            _enemyType = (EnemyType)EditorGUILayout.EnumPopup("Тип", _enemyType);
            _enemyHp = EditorGUILayout.FloatField("HP", _enemyHp);
            _enemyDamage = EditorGUILayout.FloatField("Урон", _enemyDamage);
            _enemySpeed = EditorGUILayout.FloatField("Скорость", _enemySpeed);
            if (GUILayout.Button("Создать EnemyDefinition"))
            {
                var def = ScriptableObject.CreateInstance<EnemyDefinition>();
                def.DisplayName = _enemyName;
                def.Type = _enemyType;
                def.MaxHealth = _enemyHp;
                def.ContactDamage = _enemyDamage;
                def.MoveSpeed = _enemySpeed;
                Save(def, $"Enemies/{_enemyId}.asset");
            }
        }

        private void DrawSkillSection()
        {
            EditorGUILayout.LabelField("Скилл", EditorStyles.boldLabel);
            _skillId = EditorGUILayout.TextField("Id", _skillId);
            _skillName = EditorGUILayout.TextField("Имя", _skillName);
            _skillKind = (SkillKind)EditorGUILayout.EnumPopup("Вид", _skillKind);
            _skillCooldown = EditorGUILayout.FloatField("Кулдаун", _skillCooldown);
            _skillDamage = EditorGUILayout.FloatField("Урон", _skillDamage);
            if (GUILayout.Button("Создать SkillBase"))
            {
                SkillBase skill = _skillKind switch
                {
                    SkillKind.Projectile => ScriptableObject.CreateInstance<ProjectileSkill>(),
                    SkillKind.Aoe => ScriptableObject.CreateInstance<AoESkill>(),
                    SkillKind.Orbit => ScriptableObject.CreateInstance<OrbitSkill>(),
                    SkillKind.Chain => ScriptableObject.CreateInstance<ChainSkill>(),
                    SkillKind.Nova => ScriptableObject.CreateInstance<NovaSkill>(),
                    SkillKind.Buff => ScriptableObject.CreateInstance<BuffSkill>(),
                    SkillKind.Homing => ScriptableObject.CreateInstance<HomingSkill>(),
                    _ => ScriptableObject.CreateInstance<ProjectileSkill>(),
                };
                skill.Id = _skillId;
                skill.DisplayName = _skillName;
                skill.Cooldown = _skillCooldown;
                skill.BaseDamage = _skillDamage;
                Save(skill, $"Skills/{_skillId}.asset");
            }
        }

        private void DrawTowerSection()
        {
            EditorGUILayout.LabelField("Башня", EditorStyles.boldLabel);
            _towerId = EditorGUILayout.TextField("Id", _towerId);
            _towerName = EditorGUILayout.TextField("Имя", _towerName);
            _towerType = (TowerType)EditorGUILayout.EnumPopup("Тип", _towerType);
            if (GUILayout.Button("Создать TowerDefinition"))
            {
                var def = ScriptableObject.CreateInstance<TowerDefinition>();
                def.DisplayName = _towerName;
                def.Type = _towerType;
                Save(def, $"Towers/{_towerId}.asset");
            }
        }

        private void DrawUtilitySection()
        {
            EditorGUILayout.LabelField("Утилиты", EditorStyles.boldLabel);
            if (GUILayout.Button("Открыть папку контента в Project"))
            {
                EnsureFolder(ContentRoot);
                EditorUtility.FocusProjectWindow();
                var asset = AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(ContentRoot);
                if (asset != null) Selection.activeObject = asset;
            }
            if (GUILayout.Button("Сохранить локализацию (RU/EN) в JSON"))
            {
                ExportLocalization();
            }
        }

        private static void Save(Object asset, string relativePath)
        {
            EnsureFolder(ContentRoot);
            string fullPath = Path.Combine(ContentRoot, relativePath).Replace('\\', '/');
            string dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir)) EnsureFolder(dir);
            if (AssetDatabase.LoadAssetAtPath<Object>(fullPath) != null)
            {
                fullPath = AssetDatabase.GenerateUniqueAssetPath(fullPath);
            }
            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;
        }

        private static void EnsureFolder(string folder)
        {
            if (AssetDatabase.IsValidFolder(folder)) return;
            string parent = Path.GetDirectoryName(folder)?.Replace('\\', '/');
            string leaf = Path.GetFileName(folder);
            if (!string.IsNullOrEmpty(parent) && !AssetDatabase.IsValidFolder(parent)) EnsureFolder(parent);
            AssetDatabase.CreateFolder(parent, leaf);
        }

        private static void ExportLocalization()
        {
            EnsureFolder(ContentRoot);
            string ruPath = Path.Combine(ContentRoot, "loc_ru.json").Replace('\\', '/');
            string enPath = Path.Combine(ContentRoot, "loc_en.json").Replace('\\', '/');
            File.WriteAllText(ruPath, "{\"language\":\"ru\",\"note\":\"Pre-seeded inside LocalizationManager.cs.\"}");
            File.WriteAllText(enPath, "{\"language\":\"en\",\"note\":\"Pre-seeded inside LocalizationManager.cs.\"}");
            AssetDatabase.Refresh();
        }

        public enum SkillKind
        {
            Projectile,
            Aoe,
            Orbit,
            Chain,
            Nova,
            Buff,
            Homing,
        }
    }
}
#endif
