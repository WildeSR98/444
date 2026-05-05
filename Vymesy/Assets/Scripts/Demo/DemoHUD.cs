using UnityEngine;
using Vymesy.Core;
using Vymesy.Localization;
using Vymesy.Player;
using Vymesy.Skills;
using Vymesy.Utils;

namespace Vymesy.Demo
{
    /// <summary>
    /// Minimal IMGUI HUD for the demo so it can run without TMP / font assets.
    /// Replace with the full HUDController + Canvas + TMP setup in a real scene.
    /// </summary>
    public class DemoHUD : MonoBehaviour
    {
        private GUIStyle _bigStyle;
        private GUIStyle _smallStyle;
        private Texture2D _white;
        private int _gold;

        private void OnEnable() => EventBus.Subscribe<CurrencyChangedEvent>(OnCurrency);
        private void OnDisable() => EventBus.Unsubscribe<CurrencyChangedEvent>(OnCurrency);

        private void OnCurrency(CurrencyChangedEvent evt)
        {
            if (evt.Type == CurrencyType.Gold) _gold = evt.NewAmount;
        }

        private void OnGUI()
        {
            EnsureStyles();
            if (!GameManager.HasInstance) return;
            var rm = GameManager.Instance.RunManager;
            if (rm == null) return;

            int seconds = Mathf.FloorToInt(rm.RunTime);
            string time = $"{seconds / 60:00}:{seconds % 60:00}";

            var data = GameManager.Instance.PlayerData;
            int meta = data?.MetaPoints ?? 0;
            int asc = data?.AscensionLevel ?? 0;
            GUI.Label(new Rect(20, 16, 800, 40),
                $"{Loc.T("hud.time", time)}    {Loc.T("hud.wave", rm.Wave)}    {Loc.T("hud.gold", _gold)}    meta {meta}    {Loc.T("hud.ascension", asc)}",
                _bigStyle);

            DrawHealthBar(rm);
            DrawProgressionBar(rm);
            DrawHelp();
            DrawRunOverState(rm);
        }

        private void DrawProgressionBar(RunManager rm)
        {
            var prog = rm != null ? rm.GetComponentInChildren<SkillProgressionManager>(true) : null;
            if (prog == null) return;
            var rect = new Rect(310, 60, 280, 22);
            DrawRect(rect, new Color(0.1f, 0.1f, 0.12f, 0.9f));
            float pct = prog.XPToNext > 0 ? Mathf.Clamp01(prog.CurrentXP / (float)prog.XPToNext) : 0f;
            DrawRect(new Rect(rect.x, rect.y, rect.width * pct, rect.height), new Color(0.55f, 0.85f, 1f, 0.95f));
            GUI.Label(new Rect(rect.x + 8, rect.y, rect.width, rect.height), Loc.T("hud.level", prog.Level, prog.CurrentXP, prog.XPToNext), _smallStyle);
        }

        private void DrawHealthBar(RunManager rm)
        {
            var p = rm.Player;
            if (p == null || p.Health == null) return;
            float pct = p.Health.MaxHealth > 0 ? Mathf.Clamp01(p.Health.CurrentHealth / p.Health.MaxHealth) : 0f;
            var rect = new Rect(20, 60, 280, 22);
            DrawRect(rect, new Color(0.1f, 0.1f, 0.12f, 0.9f));
            DrawRect(new Rect(rect.x, rect.y, rect.width * pct, rect.height), new Color(0.85f, 0.18f, 0.18f, 0.95f));
            GUI.Label(new Rect(rect.x + 8, rect.y, rect.width, rect.height), Loc.T("hud.hp", p.Health.CurrentHealth, p.Health.MaxHealth), _smallStyle);
        }

        private void DrawHelp()
        {
            GUI.Label(new Rect(20, Screen.height - 60, 1000, 26),
                "WASD — движение | R — рестарт | B — алтарь | F1 — статистика | LANG: " + LocalizationManager.Current,
                _smallStyle);
        }

        private void DrawRunOverState(RunManager rm)
        {
            if (rm.IsRunStarted) return;
            bool alive = rm.Player != null && rm.Player.Health != null && rm.Player.Health.IsAlive;
            string title = alive ? Loc.T("menu.start") : Loc.T("end.defeat") + " — R";
            var rect = new Rect(0, Screen.height / 2f - 24, Screen.width, 48);
            DrawRect(rect, new Color(0, 0, 0, 0.55f));
            GUI.Label(rect, title, _bigStyle);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.R) && GameManager.HasInstance)
            {
                GameManager.Instance.RunManager.StartRun();
            }
        }

        private void EnsureStyles()
        {
            if (_white == null)
            {
                _white = new Texture2D(1, 1);
                _white.SetPixel(0, 0, Color.white);
                _white.Apply();
            }
            if (_bigStyle == null)
            {
                _bigStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
                _bigStyle.normal.textColor = Color.white;
            }
            if (_smallStyle == null)
            {
                _smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 14 };
                _smallStyle.normal.textColor = new Color(0.95f, 0.95f, 0.95f);
            }
        }

        private void DrawRect(Rect rect, Color color)
        {
            var prev = GUI.color;
            GUI.color = color;
            GUI.DrawTexture(rect, _white);
            GUI.color = prev;
        }
    }
}
