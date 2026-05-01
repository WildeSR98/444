using UnityEngine;

namespace Vymesy.VFX
{
    /// <summary>
    /// Adds a fullscreen quad in front of the camera that uses the Vymesy/DarkVignette
    /// shader. Run on the main camera so the quad always covers the viewport.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class DarkVignetteOverlay : MonoBehaviour
    {
        [SerializeField] private Color _tint = new Color(0.04f, 0.03f, 0.06f);
        [SerializeField] private float _strength = 1.6f;
        [SerializeField] private float _power = 2.5f;
        [SerializeField] private float _pulseSpeed = 0.6f;
        [SerializeField] private float _pulseAmount = 0.15f;

        private GameObject _quad;
        private Material _material;
        private Camera _cam;

        private void Awake()
        {
            _cam = GetComponent<Camera>();
            var shader = Shader.Find("Vymesy/DarkVignette");
            if (shader == null) return;
            _material = new Material(shader);
            _material.SetColor("_Tint", _tint);
            _material.SetFloat("_Strength", _strength);
            _material.SetFloat("_Power", _power);
            _material.SetFloat("_PulseSpeed", _pulseSpeed);
            _material.SetFloat("_PulseAmount", _pulseAmount);

            _quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
            _quad.name = "DarkVignetteOverlay";
            Object.Destroy(_quad.GetComponent<Collider>());
            _quad.transform.SetParent(transform, false);
            _quad.GetComponent<Renderer>().sharedMaterial = _material;
            FitQuadToCamera();
        }

        private void LateUpdate()
        {
            if (_quad == null || _cam == null) return;
            FitQuadToCamera();
        }

        private void FitQuadToCamera()
        {
            float distance = Mathf.Max(_cam.nearClipPlane + 0.05f, 0.5f);
            _quad.transform.localPosition = new Vector3(0f, 0f, distance);
            float h = _cam.orthographicSize * 2f;
            float w = h * _cam.aspect;
            _quad.transform.localScale = new Vector3(w, h, 1f);
        }
    }
}
