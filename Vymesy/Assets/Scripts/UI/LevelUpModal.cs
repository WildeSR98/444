using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Skills;
using Vymesy.Utils;

namespace Vymesy.UI
{
    /// <summary>
    /// Pauses the run with <c>Time.timeScale = 0</c> when a <see cref="LevelUpEvent"/> fires
    /// and renders 3 IMGUI choices on top of the screen. Picking one applies the upgrade and
    /// resumes the run.
    /// </summary>
    public class LevelUpModal : MonoBehaviour
    {
        [SerializeField] private int _choiceCount = 3;
        [SerializeField] private List<SkillBase> _unlockPool = new List<SkillBase>();

        private readonly Queue<int> _pendingLevels = new Queue<int>();
        private List<SkillUpgradeOffer> _currentChoices;
        private float _previousTimeScale = 1f;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private Texture2D _whitePixel;

        public void SetUnlockPool(IEnumerable<SkillBase> pool)
        {
            _unlockPool.Clear();
            if (pool != null) _unlockPool.AddRange(pool);
        }

        private void OnEnable() => EventBus.Subscribe<LevelUpEvent>(HandleLevelUp);
        private void OnDisable() => EventBus.Unsubscribe<LevelUpEvent>(HandleLevelUp);

        private void HandleLevelUp(LevelUpEvent evt)
        {
            _pendingLevels.Enqueue(evt.NewLevel);
            if (_currentChoices == null) ShowNextChoice();
        }

        private void ShowNextChoice()
        {
            if (_pendingLevels.Count == 0) { Resume(); return; }
            _pendingLevels.Dequeue();

            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null || rm.Skills == null || rm.Player == null) { Resume(); return; }

            _currentChoices = SkillUpgradeFactory.Generate(rm.Skills, rm.Player, _unlockPool, _choiceCount);
            if (_currentChoices == null || _currentChoices.Count == 0) { _currentChoices = null; Resume(); return; }

            if (_previousTimeScale <= 0f) _previousTimeScale = 1f;
            else _previousTimeScale = Time.timeScale > 0f ? Time.timeScale : 1f;
            Time.timeScale = 0f;
        }

        private void OnGUI()
        {
            if (_currentChoices == null) return;
            EnsureStyles();

            float w = Mathf.Min(820, Screen.width - 40);
            float h = 320;
            var rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
            DrawRect(rect, new Color(0.04f, 0.03f, 0.06f, 0.93f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 4), new Color(0.85f, 0.55f, 0.85f));
            DrawRect(new Rect(rect.x, rect.yMax - 4, rect.width, 4), new Color(0.85f, 0.55f, 0.85f));

            GUI.Label(new Rect(rect.x, rect.y + 12, rect.width, 36), "НОВЫЙ УРОВЕНЬ — выбери силу", _titleStyle);

            int n = _currentChoices.Count;
            float card = (rect.width - 16 * (n + 1)) / Mathf.Max(1, n);
            for (int i = 0; i < n; i++)
            {
                var offer = _currentChoices[i];
                var cardRect = new Rect(rect.x + 16 + i * (card + 16), rect.y + 60, card, h - 100);
                DrawRect(cardRect, new Color(0.10f, 0.07f, 0.12f, 0.95f));
                DrawRect(new Rect(cardRect.x, cardRect.y, cardRect.width, 2), ColorForKind(offer.Kind));
                GUI.Label(new Rect(cardRect.x + 12, cardRect.y + 12, cardRect.width - 24, 36), offer.DisplayName, _titleStyle);
                GUI.Label(new Rect(cardRect.x + 12, cardRect.y + 60, cardRect.width - 24, cardRect.height - 110), offer.Description ?? string.Empty, _bodyStyle);

                if (GUI.Button(new Rect(cardRect.x + 12, cardRect.yMax - 44, cardRect.width - 24, 32), "Выбрать"))
                {
                    offer.Apply?.Invoke();
                    _currentChoices = null;
                    if (_pendingLevels.Count > 0) ShowNextChoice();
                    else Resume();
                    return;
                }
            }
        }

        private void Resume()
        {
            Time.timeScale = _previousTimeScale > 0f ? _previousTimeScale : 1f;
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
                _titleStyle = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold, wordWrap = true };
                _titleStyle.normal.textColor = new Color(0.95f, 0.9f, 0.7f);
            }
            if (_bodyStyle == null)
            {
                _bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
                _bodyStyle.normal.textColor = new Color(0.85f, 0.85f, 0.9f);
            }
        }

        private void DrawRect(Rect r, Color c)
        {
            var prev = GUI.color;
            GUI.color = c;
            GUI.DrawTexture(r, _whitePixel);
            GUI.color = prev;
        }

        private static Color ColorForKind(SkillUpgradeOffer.OfferKind k) => k switch
        {
            SkillUpgradeOffer.OfferKind.Damage => new Color(1f, 0.4f, 0.4f),
            SkillUpgradeOffer.OfferKind.Cooldown => new Color(0.5f, 0.85f, 1f),
            SkillUpgradeOffer.OfferKind.Projectiles => new Color(1f, 0.85f, 0.4f),
            SkillUpgradeOffer.OfferKind.Range => new Color(0.7f, 1f, 0.6f),
            SkillUpgradeOffer.OfferKind.NewSkill => new Color(0.9f, 0.6f, 1f),
            SkillUpgradeOffer.OfferKind.PlayerStat => new Color(1f, 0.95f, 0.6f),
            _ => Color.white,
        };
    }
}
