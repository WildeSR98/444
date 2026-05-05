using UnityEngine;

namespace Vymesy.VFX
{
    public class SpritePulse : MonoBehaviour
    {
        [SerializeField] private float _scaleAmount = 0.08f;
        [SerializeField] private float _speed = 2f;
        [SerializeField] private float _alphaAmount = 0f;

        private SpriteRenderer _renderer;
        private Vector3 _baseScale;
        private Color _baseColor;
        private float _offset;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            _baseScale = transform.localScale;
            _baseColor = _renderer != null ? _renderer.color : Color.white;
            _offset = Random.value * 10f;
        }

        private void Update()
        {
            float wave = Mathf.Sin((Time.time + _offset) * _speed);
            transform.localScale = _baseScale * (1f + wave * _scaleAmount);
            if (_renderer == null || _alphaAmount <= 0f) return;
            var c = _baseColor;
            c.a = Mathf.Clamp01(_baseColor.a + wave * _alphaAmount);
            _renderer.color = c;
        }
    }
}
