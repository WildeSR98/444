using UnityEngine;
using Vymesy.Core;
using Vymesy.Player;
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

            GUI.Label(new Rect(20, 16, 600, 40), $"Время {time}    Волна {rm.Wave}    Золото {_gold}    MetaPoints {(GameManager.Instance.PlayerData?.MetaPoints ?? 0)}", _bigStyle);

            DrawHealthBar(rm);
            DrawHelp();
            DrawRunOverState(rm);
        }

        private void DrawHealthBar(RunManager rm)
        {
            var p = rm.Player;
            if (p == null || p.Health == null) return;
            float pct = p.Health.MaxHealth > 0 ? Mathf.Clamp01(p.Health.CurrentHealth / p.Health.MaxHealth) : 0f;
            var rect = new Rect(20, 60, 280, 22);
            DrawRect(rect, new Color(0.1f, 0.1f, 0.12f, 0.9f));
            DrawRect(new Rect(rect.x, rect.y, rect.width * pct, rect.height), new Color(0.85f, 0.18f, 0.18f, 0.95f));
            GUI.Label(new Rect(rect.x + 8, rect.y, rect.width, rect.height), $"HP {Mathf.CeilToInt(p.Health.CurrentHealth)}/{Mathf.CeilToInt(p.Health.MaxHealth)}", _smallStyle);
        }

        private void DrawHelp()
        {
            GUI.Label(new Rect(20, Screen.height - 60, 800, 26), "WASD / стрелки — движение | R — респавн после смерти", _smallStyle);
        }

        private void DrawRunOverState(RunManager rm)
        {
            if (rm.IsRunStarted) return;
            string title = rm.Player != null && rm.Player.Health != null && rm.Player.Health.IsAlive ? "ЖМИ R, ЧТОБЫ ПОЙТИ В ЗАБЕГ" : "ВЫМЕСЫ ПОБЕДИЛИ — R ДЛЯ НОВОГО ЗАБЕГА";
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
