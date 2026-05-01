using UnityEngine;
using Vymesy.Core;
using Vymesy.Damage;
using Vymesy.Player;

namespace Vymesy.VFX
{
    /// <summary>
    /// Renders a shrinking telegraphed ring at a world position; on completion deals damage
    /// to the player if they are inside the radius. Uses a runtime <see cref="LineRenderer"/>
    /// so no prefabs are required.
    /// </summary>
    public class AoEWarning : MonoBehaviour
    {
        [SerializeField] private int _segments = 48;
        [SerializeField] private Color _safeColor = new Color(1f, 0.7f, 0.3f, 0.9f);
        [SerializeField] private Color _dangerColor = new Color(1f, 0.25f, 0.25f, 1f);

        private LineRenderer _line;
        private float _radius;
        private float _telegraph;
        private float _damage;
        private float _elapsed;

        public static AoEWarning Spawn(Vector3 origin, float radius, float telegraph, float damage)
        {
            var go = new GameObject("AoEWarning");
            go.transform.position = origin;
            var w = go.AddComponent<AoEWarning>();
            w.Configure(radius, telegraph, damage);
            return w;
        }

        private void Configure(float radius, float telegraph, float damage)
        {
            _radius = radius;
            _telegraph = Mathf.Max(0.05f, telegraph);
            _damage = damage;
            _line = gameObject.AddComponent<LineRenderer>();
            _line.useWorldSpace = false;
            _line.loop = true;
            _line.positionCount = _segments;
            _line.widthMultiplier = 0.08f;
            _line.material = new Material(Shader.Find("Sprites/Default"));
            _line.startColor = _safeColor;
            _line.endColor = _safeColor;
            UpdateRing();
        }

        private void Update()
        {
            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _telegraph);
            var col = Color.Lerp(_safeColor, _dangerColor, t);
            _line.startColor = col;
            _line.endColor = col;
            _line.widthMultiplier = Mathf.Lerp(0.08f, 0.22f, t);
            UpdateRing();
            if (_elapsed >= _telegraph)
            {
                Detonate();
                Destroy(gameObject);
            }
        }

        private void UpdateRing()
        {
            for (int i = 0; i < _segments; i++)
            {
                float a = (i / (float)_segments) * Mathf.PI * 2f;
                _line.SetPosition(i, new Vector3(Mathf.Cos(a) * _radius, Mathf.Sin(a) * _radius, 0f));
            }
        }

        private void Detonate()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            var player = rm != null ? rm.Player : null;
            if (player == null) return;
            float d = Vector2.Distance(player.transform.position, transform.position);
            if (d <= _radius)
            {
                player.Health?.TakeDamage(DamageInfo.Physical(_damage, gameObject));
            }
        }
    }
}
