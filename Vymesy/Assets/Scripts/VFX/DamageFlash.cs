using UnityEngine;
using Vymesy.Damage;
using Vymesy.Enemies;

namespace Vymesy.VFX
{
    /// <summary>
    /// Scale-and-color punch on damage. Plays on top of <see cref="EnemyView"/> recoloring
    /// for a more readable hit feel.
    /// </summary>
    [RequireComponent(typeof(EnemyHealth))]
    public class DamageFlash : MonoBehaviour
    {
        [SerializeField] private float _bumpScale = 1.25f;
        [SerializeField] private float _bumpDuration = 0.12f;

        private EnemyHealth _health;
        private Vector3 _baseScale;
        private float _bumpUntil;

        private void Awake()
        {
            _health = GetComponent<EnemyHealth>();
            _baseScale = transform.localScale;
        }

        private void OnEnable()
        {
            if (_health != null) _health.OnDamaged += HandleDamaged;
        }

        private void OnDisable()
        {
            if (_health != null) _health.OnDamaged -= HandleDamaged;
        }

        private void HandleDamaged(DamageInfo info)
        {
            _bumpUntil = Time.time + _bumpDuration;
        }

        private void Update()
        {
            if (Time.time < _bumpUntil)
            {
                float t = 1f - Mathf.Clamp01((_bumpUntil - Time.time) / _bumpDuration);
                float scale = Mathf.Lerp(_bumpScale, 1f, t);
                transform.localScale = _baseScale * scale;
            }
            else if (transform.localScale != _baseScale)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, _baseScale, 16f * Time.deltaTime);
                if ((transform.localScale - _baseScale).sqrMagnitude < 0.0001f) transform.localScale = _baseScale;
            }
        }
    }
}
