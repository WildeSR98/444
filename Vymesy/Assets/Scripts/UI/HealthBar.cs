using UnityEngine;
using UnityEngine.UI;
using Vymesy.Player;

namespace Vymesy.UI
{
    public class HealthBar : MonoBehaviour
    {
        [SerializeField] private Image _fill;
        [SerializeField] private PlayerHealth _health;

        public void Bind(PlayerHealth health) => _health = health;

        private void Update()
        {
            if (_fill == null || _health == null) return;
            float pct = _health.MaxHealth > 0 ? Mathf.Clamp01(_health.CurrentHealth / _health.MaxHealth) : 0f;
            _fill.fillAmount = pct;
        }
    }
}
