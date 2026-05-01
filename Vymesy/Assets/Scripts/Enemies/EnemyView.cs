using UnityEngine;

namespace Vymesy.Enemies
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class EnemyView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private float _hitFlashSeconds = 0.08f;

        private Color _baseColor;
        private float _flashUntil;

        private void Reset() => _renderer = GetComponent<SpriteRenderer>();

        private void Awake()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        }

        public void Apply(EnemyDefinition def)
        {
            if (_renderer == null) return;
            if (def != null && def.Sprite != null) _renderer.sprite = def.Sprite;
            _baseColor = def != null ? def.Tint : Color.white;
            _renderer.color = _baseColor;
            _flashUntil = 0f;
        }

        public void Flash()
        {
            if (_renderer == null) return;
            _renderer.color = Color.white;
            _flashUntil = Time.time + _hitFlashSeconds;
        }

        private void Update()
        {
            if (_renderer == null) return;
            if (_flashUntil > 0f && Time.time >= _flashUntil)
            {
                _renderer.color = _baseColor;
                _flashUntil = 0f;
            }
        }
    }
}
