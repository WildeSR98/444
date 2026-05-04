using System.Collections.Generic;
using UnityEngine;

namespace Vymesy.Localization
{
    /// <summary>
    /// Tiny in-memory localization layer. Strings are seeded in code (see <see cref="Seed"/>)
    /// so the demo doesn't need any external translation files. Two locales ship by default:
    /// <c>"ru"</c> (Russian, primary) and <c>"en"</c> (English, fallback).
    /// </summary>
    public static class LocalizationManager
    {
        public const string DefaultLanguage = "ru";

        private static readonly Dictionary<string, Dictionary<string, string>> _tables = new Dictionary<string, Dictionary<string, string>>();
        private static string _current = DefaultLanguage;
        private static bool _seeded;

        public static event System.Action OnLanguageChanged;

        public static string Current => _current;

        public static void EnsureSeeded()
        {
            if (_seeded) return;
            Seed();
            _seeded = true;
        }

        public static void SetLanguage(string lang)
        {
            EnsureSeeded();
            if (string.IsNullOrEmpty(lang) || !_tables.ContainsKey(lang)) lang = DefaultLanguage;
            if (_current == lang) return;
            _current = lang;
            OnLanguageChanged?.Invoke();
        }

        public static string T(string key)
        {
            EnsureSeeded();
            if (_tables.TryGetValue(_current, out var t) && t.TryGetValue(key, out var v)) return v;
            if (_tables.TryGetValue(DefaultLanguage, out var fallback) && fallback.TryGetValue(key, out var fv)) return fv;
            return key;
        }

        public static string T(string key, string arg0) => string.Format(T(key), arg0);
        public static string T(string key, object arg0) => string.Format(T(key), arg0);
        public static string T(string key, object arg0, object arg1) => string.Format(T(key), arg0, arg1);
        public static string T(string key, object arg0, object arg1, object arg2) => string.Format(T(key), arg0, arg1, arg2);

        public static void Register(string lang, string key, string value)
        {
            if (!_tables.TryGetValue(lang, out var t))
            {
                t = new Dictionary<string, string>();
                _tables[lang] = t;
            }
            t[key] = value;
        }

        private static void Seed()
        {
            // Russian (primary)
            R("menu.title", "ВЫМЕСЫ: ПОСЛЕДНИЙ СВЕТ");
            R("menu.start", "Начать забег");
            R("menu.stats", "Статистика");
            R("menu.altar", "Алтарь");
            R("menu.quit", "Выход");
            R("menu.lang", "Язык: {0}");

            R("hud.wave", "Волна {0}");
            R("hud.gold", "Золото {0}");
            R("hud.hp", "HP {0:0}/{1:0}");
            R("hud.time", "Время {0:0.0}");
            R("hud.kills", "Убито {0}");
            R("hud.level", "LV {0}  XP {1}/{2}");
            R("hud.ascension", "Восх. {0}");

            R("levelup.title", "НОВЫЙ УРОВЕНЬ — выбери силу");
            R("levelup.choose", "Выбрать");

            R("altar.title", "АЛТАРЬ — между забегами");
            R("altar.close", "Закрыть");
            R("altar.skip", "Пропустить");

            R("stats.title", "СТАТИСТИКА");
            R("stats.runs", "Забегов: {0}");
            R("stats.wins", "Побед: {0}");
            R("stats.highest_wave", "Лучшая волна: {0}");
            R("stats.gold_total", "Золото всего: {0}");
            R("stats.shards_total", "Осколки: {0}");
            R("stats.last_runs", "Последние забеги:");

            R("end.victory", "ВЫМЕСЫ ПОБЕЖДЕНЫ");
            R("end.defeat", "ВЫМЕСЫ ПОБЕДИЛИ");
            R("end.continue", "К алтарю");

            // English (fallback / secondary)
            E("menu.title", "VYMESY: THE LAST LIGHT");
            E("menu.start", "Start Run");
            E("menu.stats", "Statistics");
            E("menu.altar", "Altar");
            E("menu.quit", "Quit");
            E("menu.lang", "Language: {0}");

            E("hud.wave", "Wave {0}");
            E("hud.gold", "Gold {0}");
            E("hud.hp", "HP {0:0}/{1:0}");
            E("hud.time", "Time {0:0.0}");
            E("hud.kills", "Kills {0}");
            E("hud.level", "LV {0}  XP {1}/{2}");
            E("hud.ascension", "Asc. {0}");

            E("levelup.title", "LEVEL UP — choose a power");
            E("levelup.choose", "Pick");

            E("altar.title", "ALTAR — between runs");
            E("altar.close", "Close");
            E("altar.skip", "Skip");

            E("stats.title", "STATISTICS");
            E("stats.runs", "Runs: {0}");
            E("stats.wins", "Wins: {0}");
            E("stats.highest_wave", "Highest wave: {0}");
            E("stats.gold_total", "Total gold: {0}");
            E("stats.shards_total", "Soul shards: {0}");
            E("stats.last_runs", "Recent runs:");

            E("end.victory", "VYMESY DEFEATED");
            E("end.defeat", "VYMESY VICTORIOUS");
            E("end.continue", "To altar");
        }

        private static void R(string k, string v) => Register("ru", k, v);
        private static void E(string k, string v) => Register("en", k, v);
    }

    /// <summary>Convenience facade so call sites can write <c>Loc.T("hud.wave", 3)</c>.</summary>
    public static class Loc
    {
        public static string T(string key) => LocalizationManager.T(key);
        public static string T(string key, object arg0) => LocalizationManager.T(key, arg0);
        public static string T(string key, object arg0, object arg1) => LocalizationManager.T(key, arg0, arg1);
    }
}
