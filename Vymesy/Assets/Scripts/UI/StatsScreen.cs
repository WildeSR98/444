using UnityEngine;
using Vymesy.Core;
using Vymesy.Localization;
using Vymesy.Save;

namespace Vymesy.UI
{
    /// <summary>
    /// Lightweight IMGUI statistics screen. Reads aggregated counters and recent
    /// <see cref="RunHistoryEntry"/> rows from <see cref="PlayerData"/> and renders them
    /// without requiring a Canvas / TMP setup. Press F1 to toggle.
    /// </summary>
    public class StatsScreen : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private GUIStyle _smallStyle;
        private Texture2D _whitePixel;
        private float _previousTimeScale = 1f;
        private Vector2 _scroll;

        public void Open()
        {
            if (IsOpen) return;
            IsOpen = true;
            _previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
        }

        public void Close()
        {
            if (!IsOpen) return;
            IsOpen = false;
            Time.timeScale = _previousTimeScale > 0f ? _previousTimeScale : 1f;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (IsOpen) Close(); else Open();
            }
        }

        private void OnGUI()
        {
            if (!IsOpen) return;
            EnsureStyles();
            var data = GameManager.HasInstance ? GameManager.Instance.PlayerData : null;
            if (data == null) return;

            float w = Mathf.Min(720, Screen.width - 40);
            float h = Mathf.Min(560, Screen.height - 40);
            var rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
            DrawRect(rect, new Color(0.04f, 0.03f, 0.06f, 0.96f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 4), new Color(0.85f, 0.55f, 0.85f));

            GUI.Label(new Rect(rect.x, rect.y + 12, rect.width, 32), Loc.T("stats.title"), _titleStyle);

            float y = rect.y + 60;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.runs", data.RunsPlayed), _bodyStyle); y += 22;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.wins", data.RunsWon), _bodyStyle); y += 22;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.highest_wave", data.HighestWave), _bodyStyle); y += 22;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.gold_total", data.Gold), _bodyStyle); y += 22;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.shards_total", data.SoulShards), _bodyStyle); y += 22;
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22),
                $"Ascension: {data.AscensionLevel}  (highest cleared: {data.HighestAscensionCleared})", _bodyStyle); y += 28;

            // Last 12 runs.
            GUI.Label(new Rect(rect.x + 16, y, rect.width - 32, 22), Loc.T("stats.last_runs"), _titleStyle); y += 26;

            int show = Mathf.Min(12, data.RunHistory.Count);
            int start = data.RunHistory.Count - show;
            float listH = rect.yMax - y - 60;
            var listView = new Rect(rect.x + 16, y, rect.width - 32, listH);
            float entryH = 60;
            var content = new Rect(0, 0, listView.width - 20, show * entryH);
            _scroll = GUI.BeginScrollView(listView, _scroll, content);
            for (int i = 0; i < show; i++)
            {
                var entry = data.RunHistory[start + i];
                float ry = i * entryH;
                var entryRect = new Rect(0, ry, content.width, entryH - 4);
                DrawRect(entryRect, new Color(0.10f, 0.07f, 0.12f, 0.95f));
                DrawRect(new Rect(entryRect.x, entryRect.y, 4, entryRect.height),
                    entry.Victory ? new Color(0.4f, 0.95f, 0.5f) : new Color(0.95f, 0.4f, 0.4f));
                GUI.Label(new Rect(entryRect.x + 12, entryRect.y + 4, entryRect.width - 24, 22),
                    $"Wave {entry.WaveReached}   Kills {entry.EnemiesKilled}   Gold {entry.GoldCollected}   {(entry.Victory ? "VICTORY" : "DEFEAT")}", _bodyStyle);
                GUI.Label(new Rect(entryRect.x + 12, entryRect.y + 26, entryRect.width - 24, 18),
                    $"{entry.DurationSeconds:0.0}s   asc {entry.AscensionLevel}   {EpochToTime(entry.EndTimeUnix)}", _smallStyle);
            }
            GUI.EndScrollView();

            // Bottom DPS bar chart for last run.
            if (data.RunHistory.Count > 0)
            {
                var last = data.RunHistory[data.RunHistory.Count - 1];
                float dps = last.DurationSeconds > 0.01f ? last.EnemiesKilled / last.DurationSeconds : 0f;
                GUI.Label(new Rect(rect.x + 16, rect.yMax - 56, rect.width - 32, 22),
                    $"Last run avg kills/s: {dps:0.00}   gold/min: {(last.GoldCollected / Mathf.Max(0.1f, last.DurationSeconds / 60f)):0}",
                    _bodyStyle);
            }

            if (GUI.Button(new Rect(rect.xMax - 140, rect.yMax - 44, 120, 32), Loc.T("altar.close")))
            {
                Close();
            }
        }

        private static string EpochToTime(long unix)
        {
            if (unix <= 0) return "—";
            var dt = System.DateTimeOffset.FromUnixTimeSeconds(unix).LocalDateTime;
            return dt.ToString("yyyy-MM-dd HH:mm");
        }

        private void EnsureStyles()
        {
            if (_whitePixel == null)
            {
                _whitePixel = new Texture2D(1, 1);
                _whitePixel.SetPixel(0, 0, Color.white);
                _whitePixel.Apply();
            }
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold };
                _titleStyle.normal.textColor = new Color(0.95f, 0.9f, 0.7f);
            }
            if (_bodyStyle == null)
            {
                _bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
                _bodyStyle.normal.textColor = new Color(0.85f, 0.85f, 0.9f);
            }
            if (_smallStyle == null)
            {
                _smallStyle = new GUIStyle(GUI.skin.label) { fontSize = 11 };
                _smallStyle.normal.textColor = new Color(0.6f, 0.6f, 0.7f);
            }
        }

        private void DrawRect(Rect r, Color c)
        {
            var prev = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, _whitePixel);
            GUI.color = prev;
        }
    }
}
