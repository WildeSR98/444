using UnityEngine;
using Vymesy.Core;
using Vymesy.Damage;
using Vymesy.Demo;
using Vymesy.Player;

namespace Vymesy.Biome
{
    /// <summary>
    /// Generic ground hazard. Behaviour switches on <see cref="Kind"/>: lava ticks damage,
    /// fog/ice slow, shadow vent does small damage and reduces visibility.
    /// </summary>
    public class Hazard : MonoBehaviour
    {
        public BiomeHazard Kind;
        public float Radius = 1.4f;
        public float Damage = 4f;
        public float DamageInterval = 0.6f;

        private float _nextDamageTime;
        private SpriteRenderer _sr;

        public static Hazard Spawn(BiomeHazard kind, Vector3 position, float radius, Color color, float damage)
        {
            var go = new GameObject($"Hazard_{kind}");
            go.transform.position = position;
            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite = DemoSprites.Get(DemoSprites.Shape.Circle, color, 64);
            sr.sortingOrder = 1;
            float scale = radius * 2f / 1f; // sprite radius ~ 0.5 unit
            go.transform.localScale = Vector3.one * scale;
            var h = go.AddComponent<Hazard>();
            h.Kind = kind;
            h.Radius = radius;
            h.Damage = damage;
            h._sr = sr;
            return h;
        }

        private void Update()
        {
            var rm = GameManager.HasInstance ? GameManager.Instance.RunManager : null;
            var player = rm != null ? rm.Player : null;
            if (player == null) return;

            float dist = Vector2.Distance(player.transform.position, transform.position);
            if (dist > Radius) return;

            switch (Kind)
            {
                case BiomeHazard.Lava:
                case BiomeHazard.ShadowVent:
                    if (Time.time >= _nextDamageTime)
                    {
                        player.Health?.TakeDamage(DamageInfo.Fire(Damage, gameObject));
                        _nextDamageTime = Time.time + DamageInterval;
                    }
                    break;
                case BiomeHazard.Ice:
                    // Apply a temporary slow by piggybacking on a stat modifier; clears
                    // automatically on exit because we restore stats in LateUpdate.
                    BiomeIceDecal.Refresh(player);
                    break;
                case BiomeHazard.Fog:
                    BiomeFogDecal.Refresh(player);
                    break;
            }
        }

        // Make the colour pulse so hazards read clearly even in dense screens.
        private void LateUpdate()
        {
            if (_sr == null) return;
            float p = 0.5f + 0.5f * Mathf.Sin(Time.time * 2.5f);
            var col = _sr.color;
            col.a = Mathf.Lerp(0.55f, 0.95f, p);
            _sr.color = col;
        }
    }

    /// <summary>Marker component re-applied while the player stands in fog.</summary>
    public class BiomeFogDecal : MonoBehaviour
    {
        private float _expireTime;
        private static BiomeFogDecal _active;

        public static void Refresh(PlayerManager p)
        {
            if (_active == null)
            {
                _active = p.gameObject.AddComponent<BiomeFogDecal>();
            }
            _active._expireTime = Time.time + 0.3f;
        }

        private void Update()
        {
            if (Time.time > _expireTime)
            {
                Destroy(this);
                _active = null;
            }
        }
    }

    /// <summary>Marker component re-applied while the player stands in ice — halves move speed.</summary>
    public class BiomeIceDecal : MonoBehaviour
    {
        private float _expireTime;
        private PlayerStatsModifier _mod;
        private static BiomeIceDecal _active;

        public static void Refresh(PlayerManager p)
        {
            if (_active == null)
            {
                _active = p.gameObject.AddComponent<BiomeIceDecal>();
                _active._mod = new PlayerStatsModifier { MoveSpeed = -2f };
                p.AddModifier(_active._mod);
            }
            _active._expireTime = Time.time + 0.3f;
        }

        private void OnDestroy()
        {
            var p = GetComponent<PlayerManager>();
            if (p != null && _mod != null) p.RemoveModifier(_mod);
            if (_active == this) _active = null;
        }

        private void Update()
        {
            if (Time.time > _expireTime)
            {
                Destroy(this);
            }
        }
    }
}
