using System.Collections.Generic;
using UnityEngine;
using Vymesy.Core;
using Vymesy.Player;
using Vymesy.Save;
using Vymesy.Utils;

namespace Vymesy.UI
{
    /// <summary>
    /// IMGUI shop that opens between runs. The player spends Gold/MetaPoints/SoulShards on
    /// transient buffs that activate on the next run start (consumed by
    /// <see cref="PlayerManager.ResetForRun"/>) or on permanent unlocks (Ascension level,
    /// language switch, etc.).
    /// </summary>
    public class AltarShop : MonoBehaviour
    {
        public bool IsOpen { get; private set; }

        [SerializeField] private List<ShopOffer> _offers = new List<ShopOffer>();
        private float _previousTimeScale = 1f;
        private GUIStyle _titleStyle;
        private GUIStyle _bodyStyle;
        private Texture2D _whitePixel;

        private void Awake()
        {
            BuildDefaultOffers();
        }

        private void OnEnable()
        {
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnded);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnded);
        }

        private void HandleRunEnded(RunEndedEvent _)
        {
            // Auto-open the altar between runs so the loop is "die → spend → restart".
            Open();
        }

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
            // Persist purchases.
            if (GameManager.HasInstance && GameManager.Instance.PlayerData != null)
                SaveLoadManager.Save(GameManager.Instance.PlayerData);
        }

        private void Update()
        {
            // Press B to toggle the altar at any time.
            if (Input.GetKeyDown(KeyCode.B))
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

            float w = Mathf.Min(880, Screen.width - 40);
            float h = Mathf.Min(560, Screen.height - 40);
            var rect = new Rect((Screen.width - w) * 0.5f, (Screen.height - h) * 0.5f, w, h);
            DrawRect(rect, new Color(0.04f, 0.03f, 0.06f, 0.96f));
            DrawRect(new Rect(rect.x, rect.y, rect.width, 4), new Color(0.85f, 0.55f, 0.85f));

            GUI.Label(new Rect(rect.x, rect.y + 12, rect.width, 32), "АЛТАРЬ — между забегами", _titleStyle);
            GUI.Label(new Rect(rect.x + 16, rect.y + 50, rect.width - 32, 22),
                $"Gold {data.Gold}    MetaPoints {data.MetaPoints}    SoulShards {data.SoulShards}    Ascension {data.AscensionLevel}", _bodyStyle);

            float listY = rect.y + 86;
            float row = 64;
            for (int i = 0; i < _offers.Count; i++)
            {
                var offer = _offers[i];
                var rowRect = new Rect(rect.x + 16, listY + i * (row + 6), rect.width - 32, row);
                DrawRect(rowRect, new Color(0.10f, 0.07f, 0.12f, 0.95f));
                GUI.Label(new Rect(rowRect.x + 12, rowRect.y + 6, rowRect.width - 200, 22), offer.DisplayName, _titleStyle);
                GUI.Label(new Rect(rowRect.x + 12, rowRect.y + 28, rowRect.width - 200, rowRect.height - 30), offer.Description, _bodyStyle);

                bool canAfford = CanAfford(data, offer);
                string label = $"{offer.Cost} {CurrencyLabel(offer.Currency)}";
                GUI.enabled = canAfford;
                if (GUI.Button(new Rect(rowRect.xMax - 180, rowRect.y + 16, 168, 32), $"Купить — {label}"))
                {
                    Spend(data, offer);
                    offer.Apply?.Invoke(data);
                }
                GUI.enabled = true;
            }

            if (GUI.Button(new Rect(rect.xMax - 140, rect.yMax - 44, 120, 32), "Закрыть"))
            {
                Close();
            }
        }

        private void BuildDefaultOffers()
        {
            _offers.Clear();
            _offers.Add(new ShopOffer
            {
                Id = "next_hp",
                DisplayName = "Эликсир жизни",
                Description = "Старт следующего забега с +30 HP и +1 hp/сек реген.",
                Cost = 30, Currency = ShopCurrency.Gold,
                Apply = data => data.NextRunBoosts.Add(new PlayerStatsModifier { MaxHealth = 30, HealthRegenPerSecond = 1f }),
            });
            _offers.Add(new ShopOffer
            {
                Id = "next_dmg",
                DisplayName = "Тоник ярости",
                Description = "Старт следующего забега с +20% урона.",
                Cost = 50, Currency = ShopCurrency.Gold,
                Apply = data => data.NextRunBoosts.Add(new PlayerStatsModifier { DamageMultiplier = 0.20f }),
            });
            _offers.Add(new ShopOffer
            {
                Id = "next_speed",
                DisplayName = "Феятный нектар",
                Description = "Старт следующего забега с +1 MoveSpeed и +0.5 PickupRadius.",
                Cost = 40, Currency = ShopCurrency.Gold,
                Apply = data => data.NextRunBoosts.Add(new PlayerStatsModifier { MoveSpeed = 1f, PickupRadius = 0.5f }),
            });
            _offers.Add(new ShopOffer
            {
                Id = "next_crit",
                DisplayName = "Кристалл прицела",
                Description = "Старт следующего забега с +10% крита и +0.5 крит-урона.",
                Cost = 4, Currency = ShopCurrency.SoulShards,
                Apply = data => data.NextRunBoosts.Add(new PlayerStatsModifier { CritChance = 0.10f, CritMultiplier = 0.5f }),
            });
            _offers.Add(new ShopOffer
            {
                Id = "ascend",
                DisplayName = "Восхождение (NG+)",
                Description = "Постоянно: +1 уровень восхождения. Враги становятся прочнее, дроп богаче, игрок получает +10 HP/+5% урона за уровень.",
                Cost = 5, Currency = ShopCurrency.MetaPoints,
                Apply = data => data.AscensionLevel++,
            });
            _offers.Add(new ShopOffer
            {
                Id = "lang_toggle",
                DisplayName = "Сменить язык RU/EN",
                Description = "Переключить локализацию между русским и английским.",
                Cost = 0, Currency = ShopCurrency.Gold,
                Apply = data =>
                {
                    data.Language = data.Language == "ru" ? "en" : "ru";
                    Localization.LocalizationManager.SetLanguage(data.Language);
                },
            });
            _offers.Add(new ShopOffer
            {
                Id = "lang_toggle_dummy",
                DisplayName = "Пропустить",
                Description = "Закрыть алтарь без покупок и продолжить.",
                Cost = 0, Currency = ShopCurrency.Gold,
                Apply = _ => Close(),
            });
        }

        private static bool CanAfford(PlayerData data, ShopOffer offer)
        {
            return offer.Currency switch
            {
                ShopCurrency.Gold => data.Gold >= offer.Cost,
                ShopCurrency.MetaPoints => data.MetaPoints >= offer.Cost,
                ShopCurrency.SoulShards => data.SoulShards >= offer.Cost,
                _ => false,
            };
        }

        private static void Spend(PlayerData data, ShopOffer offer)
        {
            switch (offer.Currency)
            {
                case ShopCurrency.Gold: data.Gold -= offer.Cost; break;
                case ShopCurrency.MetaPoints: data.MetaPoints -= offer.Cost; break;
                case ShopCurrency.SoulShards: data.SoulShards -= offer.Cost; break;
            }
        }

        private static string CurrencyLabel(ShopCurrency c) => c switch
        {
            ShopCurrency.Gold => "gold",
            ShopCurrency.MetaPoints => "meta",
            ShopCurrency.SoulShards => "shards",
            _ => "?",
        };

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
                _bodyStyle = new GUIStyle(GUI.skin.label) { fontSize = 13, wordWrap = true };
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
    }

    [System.Serializable]
    public class ShopOffer
    {
        public string Id;
        public string DisplayName;
        [TextArea] public string Description;
        public int Cost;
        public ShopCurrency Currency;
        public System.Action<PlayerData> Apply;
    }

    public enum ShopCurrency
    {
        Gold,
        MetaPoints,
        SoulShards,
    }
}
