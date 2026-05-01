using UnityEngine;

namespace Vymesy.VFX
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _smoothTime = 0.12f;
        [SerializeField] private Vector3 _offset = new Vector3(0f, 0f, -10f);

        private Vector3 _velocity;

        public void SetTarget(Transform target) => _target = target;

        private void LateUpdate()
        {
            if (_target == null) return;
            Vector3 desired = _target.position + _offset;
            transform.position = Vector3.SmoothDamp(transform.position, desired, ref _velocity, _smoothTime);
        }
    }
}
