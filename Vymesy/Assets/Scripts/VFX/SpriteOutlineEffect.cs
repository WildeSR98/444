using UnityEngine;

namespace Vymesy.VFX
{
    /// <summary>
    /// Swaps the SpriteRenderer's material with one using the Vymesy/SpriteOutline shader.
    /// Add to bosses, elites or hovered enemies for a glowing edge.
    /// </summary>
    [RequireComponent(typeof(SpriteRenderer))]
    public class SpriteOutlineEffect : MonoBehaviour
    {
        [SerializeField] private Color _outlineColor = new Color(1f, 0.85f, 0.4f);
        [SerializeField, Range(0, 8)] private float _thickness = 2f;
        [SerializeField, Range(0, 6)] private float _pulseSpeed = 2f;

        private SpriteRenderer _renderer;
        private Material _material;

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
            var shader = Shader.Find("Vymesy/SpriteOutline");
            if (shader == null) return;
            _material = new Material(shader);
            _material.SetColor("_OutlineColor", _outlineColor);
            _material.SetFloat("_OutlineThickness", _thickness);
            _material.SetFloat("_OutlinePulseSpeed", _pulseSpeed);
            _renderer.sharedMaterial = _material;
        }

        public void SetColor(Color c)
        {
            _outlineColor = c;
            if (_material != null) _material.SetColor("_OutlineColor", c);
        }
    }
}
