using UnityEngine;
using Vymesy.Player;

namespace Vymesy.UI
{
    /// <summary>
    /// IMGUI virtual joystick. Activates only on touch / mobile (or when <c>Force = true</c>),
    /// renders a base ring + draggable thumb in the bottom-left of the screen, and exposes
    /// the resulting normalised direction via <see cref="ITouchInputSource.Read"/>.
    /// </summary>
    public class TouchJoystick : MonoBehaviour, ITouchInputSource
    {
        [SerializeField] private float _baseRadius = 90f;
        [SerializeField] private float _thumbRadius = 36f;
        [SerializeField] private Vector2 _screenAnchor = new Vector2(160, 160);
        [SerializeField] private Color _baseColor = new Color(1f, 1f, 1f, 0.18f);
        [SerializeField] private Color _thumbColor = new Color(1f, 1f, 1f, 0.55f);
        [SerializeField] private bool _force;

        public bool Force { get => _force; set => _force = value; }

        private bool _active;
        private int _trackedFingerId = -1;
        private Vector2 _origin;
        private Vector2 _current;
        private Texture2D _whitePixel;

        public Vector2 Read()
        {
            if (!_active) return Vector2.zero;
            Vector2 delta = _current - _origin;
            float r = Mathf.Max(1f, _baseRadius);
            Vector2 norm = Vector2.ClampMagnitude(delta / r, 1f);
            return norm;
        }

        private bool IsTouchActive()
        {
            if (_force) return true;
#if UNITY_ANDROID || UNITY_IOS
            return Application.isMobilePlatform;
#else
            return Input.touchSupported && Input.touchCount > 0;
#endif
        }

        private void Update()
        {
            if (!IsTouchActive())
            {
                _active = false;
                _trackedFingerId = -1;
                return;
            }
            // Touch tracking — find a finger in the bottom-left quadrant and follow it.
            for (int i = 0; i < Input.touchCount; i++)
            {
                var touch = Input.GetTouch(i);
                Vector2 pos = touch.position;
                if (touch.phase == TouchPhase.Began && _trackedFingerId == -1 && pos.x < Screen.width * 0.5f)
                {
                    _trackedFingerId = touch.fingerId;
                    _active = true;
                    _origin = pos;
                    _current = pos;
                }
                else if (touch.fingerId == _trackedFingerId)
                {
                    _current = pos;
                    if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                    {
                        _active = false;
                        _trackedFingerId = -1;
                    }
                }
            }
        }

        private void OnGUI()
        {
            if (!IsTouchActive()) return;
            EnsureWhite();
            Vector2 origin = _active ? new Vector2(_origin.x, Screen.height - _origin.y) : _screenAnchor;
            Vector2 thumb = _active ? new Vector2(_current.x, Screen.height - _current.y) : origin;
            // Clamp thumb visually inside the base ring.
            Vector2 dir = thumb - origin;
            if (dir.magnitude > _baseRadius) thumb = origin + dir.normalized * _baseRadius;

            DrawCircle(origin, _baseRadius, _baseColor, 32);
            DrawCircle(thumb, _thumbRadius, _thumbColor, 24);
        }

        private void EnsureWhite()
        {
            if (_whitePixel != null) return;
            _whitePixel = new Texture2D(1, 1);
            _whitePixel.SetPixel(0, 0, Color.white);
            _whitePixel.Apply();
        }

        private void DrawCircle(Vector2 center, float radius, Color color, int segments)
        {
            // Stamp small textured rectangles around the perimeter — cheap fake circle.
            var prev = GUI.color;
            GUI.color = color;
            // Filled disk via concentric arcs
            float step = 2f;
            for (float r = radius * 0.2f; r <= radius; r += step)
            {
                int seg = Mathf.Max(8, segments);
                for (int i = 0; i < seg; i++)
                {
                    float a = (i / (float)seg) * Mathf.PI * 2f;
                    Vector2 p = center + new Vector2(Mathf.Cos(a), Mathf.Sin(a)) * r;
                    GUI.DrawTexture(new Rect(p.x - 1, p.y - 1, 2, 2), _whitePixel);
                }
            }
            GUI.color = prev;
        }
    }
}
