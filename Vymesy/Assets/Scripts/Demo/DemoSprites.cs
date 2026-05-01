using UnityEngine;

namespace Vymesy.Demo
{
    /// <summary>
    /// Generates simple programmatic sprites so the demo doesn't require any imported textures.
    /// Sprites are cached per (size, shape, color) to avoid GC pressure.
    /// </summary>
    public static class DemoSprites
    {
        private static readonly System.Collections.Generic.Dictionary<int, Sprite> _cache = new();

        public enum Shape { Circle, Square, Diamond, Cross }

        public static Sprite Get(Shape shape, Color color, int size = 32)
        {
            int key = HashCode(shape, color, size);
            if (_cache.TryGetValue(key, out var existing) && existing != null) return existing;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var pixels = new Color32[size * size];
            float r = (size - 1) * 0.5f;
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                bool inside = false;
                float dx = x - r, dy = y - r;
                switch (shape)
                {
                    case Shape.Circle:
                        inside = (dx * dx + dy * dy) <= r * r;
                        break;
                    case Shape.Square:
                        inside = Mathf.Abs(dx) < r * 0.95f && Mathf.Abs(dy) < r * 0.95f;
                        break;
                    case Shape.Diamond:
                        inside = (Mathf.Abs(dx) + Mathf.Abs(dy)) <= r;
                        break;
                    case Shape.Cross:
                        float t = r * 0.3f;
                        inside = Mathf.Abs(dx) < t || Mathf.Abs(dy) < t;
                        break;
                }
                pixels[y * size + x] = inside ? (Color32)color : new Color32(0, 0, 0, 0);
            }
            tex.SetPixels32(pixels);
            tex.Apply(false, true);
            var sprite = Sprite.Create(tex, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
            sprite.name = $"DemoSprite_{shape}_{ColorUtility.ToHtmlStringRGBA(color)}_{size}";
            _cache[key] = sprite;
            return sprite;
        }

        private static int HashCode(Shape shape, Color color, int size)
        {
            unchecked
            {
                int h = (int)shape * 397;
                h ^= color.GetHashCode();
                h = h * 397 ^ size;
                return h;
            }
        }
    }
}
