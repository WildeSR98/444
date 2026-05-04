using UnityEngine;
using UnityEngine.UI;
using Vymesy.Core;
using Vymesy.Player;
using Vymesy.Utils;

namespace Vymesy.UI
{
    public class HUDController : MonoBehaviour
    {
        [SerializeField] private Text _goldText;
        [SerializeField] private Text _waveText;
        [SerializeField] private Text _timerText;
        [SerializeField] private HealthBar _healthBar;

        private void OnEnable()
        {
            EventBus.Subscribe<CurrencyChangedEvent>(HandleCurrency);
            EventBus.Subscribe<WaveStartedEvent>(HandleWave);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<CurrencyChangedEvent>(HandleCurrency);
            EventBus.Unsubscribe<WaveStartedEvent>(HandleWave);
        }

        private void Start()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm != null && rm.Player != null && _healthBar != null)
            {
                _healthBar.Bind(rm.Player.Health);
            }
        }

        private void Update()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            if (rm == null) return;
            if (_timerText != null)
            {
                int seconds = Mathf.FloorToInt(rm.RunTime);
                _timerText.text = $"{seconds / 60:00}:{seconds % 60:00}";
            }
            if (_waveText != null) _waveText.text = $"Волна {rm.Wave}";
        }

        private void HandleCurrency(CurrencyChangedEvent evt)
        {
            if (_goldText != null && evt.Type == CurrencyType.Gold)
                _goldText.text = evt.NewAmount.ToString();
        }

        private void HandleWave(WaveStartedEvent evt)
        {
            if (_waveText != null) _waveText.text = $"Волна {evt.Wave}";
        }
    }
}
