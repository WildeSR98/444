using UnityEngine;

namespace Vymesy.Player
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;
        [SerializeField] private float _inputDeadzone = 0.05f;

        private PlayerStats _stats;
        private Vector2 _moveInput;
        private Vector2 _aimDirection = Vector2.right;

        public Vector2 AimDirection => _aimDirection;
        public Vector2 Velocity => _rb != null ? GetRbVelocity() : Vector2.zero;

        // Optional virtual joystick (mobile). When set, its <see cref="ITouchInputSource.Read"/>
        // is summed with keyboard input so the same controller works on PC and touch.
        public ITouchInputSource TouchInput { get; set; }

        private Vector2 GetRbVelocity()
        {
#if UNITY_6000_0_OR_NEWER
            return _rb.linearVelocity;
#else
            return _rb.velocity;
#endif
        }

        private void SetRbVelocity(Vector2 v)
        {
#if UNITY_6000_0_OR_NEWER
            _rb.linearVelocity = v;
#else
            _rb.velocity = v;
#endif
        }

        public void Bind(PlayerStats stats) => _stats = stats;

        private void Reset() => _rb = GetComponent<Rigidbody2D>();

        private void Awake()
        {
            if (_rb == null) _rb = GetComponent<Rigidbody2D>();
            if (_rb != null)
            {
                _rb.gravityScale = 0f;
                _rb.freezeRotation = true;
            }
        }

        private void Update()
        {
            float h = Input.GetAxisRaw("Horizontal");
            float v = Input.GetAxisRaw("Vertical");
            Vector2 input = new Vector2(h, v);
            if (TouchInput != null)
            {
                Vector2 t = TouchInput.Read();
                if (t.sqrMagnitude > input.sqrMagnitude) input = t;
            }
            if (input.sqrMagnitude > 1f) input.Normalize();
            _moveInput = input;
            if (_moveInput.sqrMagnitude > _inputDeadzone * _inputDeadzone)
            {
                _aimDirection = _moveInput.normalized;
            }
        }

        private void FixedUpdate()
        {
            if (_rb == null || _stats == null) return;
            SetRbVelocity(_moveInput * _stats.MoveSpeed);
        }
    }
}
