using UnityEngine;
using Vymesy.Utils;

namespace Vymesy.VFX
{
    /// <summary>
    /// Reactive camera shake. Add to the main camera and adjust amplitudes per event.
    /// </summary>
    public class CameraShake : MonoBehaviour
    {
        [SerializeField] private float _hitAmplitude = 0.05f;
        [SerializeField] private float _hitDuration = 0.08f;
        [SerializeField] private float _runEndAmplitude = 0.2f;
        [SerializeField] private float _runEndDuration = 0.4f;

        private Vector3 _basePos;
        private float _shakeUntil;
        private float _amplitude;
        private float _seedX, _seedY;

        private void Awake() => _basePos = transform.localPosition;

        private void OnEnable()
        {
            EventBus.Subscribe<PlayerDamagedEvent>(HandleHit);
            EventBus.Subscribe<RunEndedEvent>(HandleRunEnd);
        }

        private void OnDisable()
        {
            EventBus.Unsubscribe<PlayerDamagedEvent>(HandleHit);
            EventBus.Unsubscribe<RunEndedEvent>(HandleRunEnd);
        }

        private void HandleHit(PlayerDamagedEvent _) => Trigger(_hitAmplitude, _hitDuration);
        private void HandleRunEnd(RunEndedEvent _) => Trigger(_runEndAmplitude, _runEndDuration);

        private void Trigger(float amplitude, float duration)
        {
            _amplitude = amplitude;
            _shakeUntil = Time.time + duration;
            _seedX = Random.value * 100f;
            _seedY = Random.value * 100f;
        }

        private void Update()
        {
            if (Time.time < _shakeUntil)
            {
                float t = Time.time * 30f;
                float dx = (Mathf.PerlinNoise(_seedX + t, 0f) - 0.5f) * 2f;
                float dy = (Mathf.PerlinNoise(0f, _seedY + t) - 0.5f) * 2f;
                transform.localPosition = _basePos + new Vector3(dx, dy, 0f) * _amplitude;
            }
            else if (transform.localPosition != _basePos)
            {
                transform.localPosition = Vector3.Lerp(transform.localPosition, _basePos, 12f * Time.deltaTime);
                if ((transform.localPosition - _basePos).sqrMagnitude < 0.0001f) transform.localPosition = _basePos;
            }
        }
    }
}
