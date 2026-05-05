using UnityEngine;

namespace Vymesy.Demo
{
    /// <summary>
    /// Generates small procedural pixel sprites so the demo doesn't require imported textures.
    /// Sprites are cached per (size, shape, color) to avoid GC pressure.
    /// </summary>
    public static class DemoSprites
    {
        private static readonly System.Collections.Generic.Dictionary<int, Sprite> _cache = new();

        public enum Shape
        {
            Circle,
            Square,
            Diamond,
            Cross,
            Player,
            CommonVymes,
            StalkerVymes,
            BruteVymes,
            WretchVymes,
            EliteVymes,
            BossVymes,
            ShinyVymes,
            TowerAltar,
            TowerArcher,
            TowerPoison,
            TowerStorm,
            TowerAura,
            Chest,
            Bolt,
            Orb,
            Needle,
            EnemyBolt
        }

        public static Sprite Get(Shape shape, Color color, int size = 32)
        {
            int key = HashCode(shape, color, size);
            if (_cache.TryGetValue(key, out var existing) && existing != null) return existing;

            var tex = new Texture2D(size, size, TextureFormat.RGBA32, false) { filterMode = FilterMode.Point };
            var pixels = new Color32[size * size];
            Draw(shape, pixels, size, color);
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

        private static void Draw(Shape shape, Color32[] pixels, int size, Color color)
        {
            switch (shape)
            {
                case Shape.Player:
                    DrawPlayer(pixels, size, color);
                    break;
                case Shape.CommonVymes:
                    DrawCommonVymes(pixels, size, color);
                    break;
                case Shape.StalkerVymes:
                    DrawStalkerVymes(pixels, size, color);
                    break;
                case Shape.BruteVymes:
                    DrawBruteVymes(pixels, size, color);
                    break;
                case Shape.WretchVymes:
                    DrawWretchVymes(pixels, size, color);
                    break;
                case Shape.EliteVymes:
                    DrawEliteVymes(pixels, size, color);
                    break;
                case Shape.BossVymes:
                    DrawBossVymes(pixels, size, color);
                    break;
                case Shape.ShinyVymes:
                    DrawShinyVymes(pixels, size, color);
                    break;
                case Shape.TowerAltar:
                    DrawTowerAltar(pixels, size, color);
                    break;
                case Shape.TowerArcher:
                    DrawTowerArcher(pixels, size, color);
                    break;
                case Shape.TowerPoison:
                    DrawTowerPoison(pixels, size, color);
                    break;
                case Shape.TowerStorm:
                    DrawTowerStorm(pixels, size, color);
                    break;
                case Shape.TowerAura:
                    DrawTowerAura(pixels, size, color);
                    break;
                case Shape.Chest:
                    DrawChest(pixels, size, color);
                    break;
                case Shape.Bolt:
                    DrawBolt(pixels, size, color);
                    break;
                case Shape.Orb:
                    DrawOrb(pixels, size, color);
                    break;
                case Shape.Needle:
                    DrawNeedle(pixels, size, color);
                    break;
                case Shape.EnemyBolt:
                    DrawEnemyBolt(pixels, size, color);
                    break;
                default:
                    DrawBasic(shape, pixels, size, color);
                    break;
            }
        }

        private static void DrawBasic(Shape shape, Color32[] pixels, int size, Color color)
        {
            float r = (size - 1) * 0.5f;
            Color outline = Shade(color, 0.32f);
            for (int y = 0; y < size; y++)
            for (int x = 0; x < size; x++)
            {
                float dx = x - r, dy = y - r;
                bool inside = false;
                bool inner = false;
                switch (shape)
                {
                    case Shape.Circle:
                        inside = dx * dx + dy * dy <= r * r;
                        inner = dx * dx + dy * dy <= (r - 2f) * (r - 2f);
                        break;
                    case Shape.Square:
                        inside = Mathf.Abs(dx) < r * 0.95f && Mathf.Abs(dy) < r * 0.95f;
                        inner = Mathf.Abs(dx) < r * 0.72f && Mathf.Abs(dy) < r * 0.72f;
                        break;
                    case Shape.Diamond:
                        inside = Mathf.Abs(dx) + Mathf.Abs(dy) <= r;
                        inner = Mathf.Abs(dx) + Mathf.Abs(dy) <= r - 3f;
                        break;
                    case Shape.Cross:
                        float t = r * 0.3f;
                        inside = Mathf.Abs(dx) < t || Mathf.Abs(dy) < t;
                        inner = Mathf.Abs(dx) < t - 2f || Mathf.Abs(dy) < t - 2f;
                        break;
                }
                if (inside) Set(pixels, size, x, y, inner ? color : outline);
            }
        }

        private static void DrawPlayer(Color32[] p, int s, Color c)
        {
            Color outline = new Color(0.2f, 0.18f, 0.12f, 1f);
            Color hood = Shade(c, 0.72f);
            Color face = new Color(1f, 0.86f, 0.56f, 1f);
            FillCircle(p, s, 16, 16, 12, outline);
            FillCircle(p, s, 16, 16, 10, hood);
            FillCircle(p, s, 16, 15, 6, face);
            FillRect(p, s, 13, 11, 15, 13, new Color(0.08f, 0.08f, 0.1f, 1f));
            FillRect(p, s, 18, 11, 20, 13, new Color(0.08f, 0.08f, 0.1f, 1f));
            FillDiamond(p, s, 16, 5, 4, new Color(1f, 0.9f, 0.3f, 1f));
            FillRect(p, s, 15, 22, 17, 29, new Color(0.95f, 0.82f, 0.28f, 1f));
        }

        private static void DrawCommonVymes(Color32[] p, int s, Color c)
        {
            DrawBlob(p, s, c, 11, 12, horned: false);
            DrawEyes(p, s, 11, 13, 20, 13, Color.red);
        }

        private static void DrawStalkerVymes(Color32[] p, int s, Color c)
        {
            FillDiamond(p, s, 16, 16, 15, Shade(c, 0.25f));
            FillDiamond(p, s, 16, 16, 12, c);
            FillRect(p, s, 10, 13, 22, 15, new Color(0.08f, 0f, 0.08f, 1f));
            DrawEyes(p, s, 12, 13, 20, 13, new Color(1f, 0.25f, 0.85f, 1f));
        }

        private static void DrawBruteVymes(Color32[] p, int s, Color c)
        {
            DrawBlob(p, s, c, 13, 10, horned: true);
            FillRect(p, s, 9, 19, 23, 22, Shade(c, 0.45f));
            DrawEyes(p, s, 11, 13, 20, 13, new Color(1f, 0.2f, 0.1f, 1f));
        }

        private static void DrawWretchVymes(Color32[] p, int s, Color c)
        {
            FillCircle(p, s, 16, 17, 12, Shade(c, 0.25f));
            FillCircle(p, s, 16, 17, 10, c);
            FillRect(p, s, 13, 5, 19, 19, Shade(c, 1.35f));
            FillCircle(p, s, 16, 7, 4, new Color(0.55f, 1f, 0.45f, 1f));
            DrawEyes(p, s, 11, 15, 20, 15, new Color(0.2f, 1f, 0.35f, 1f));
        }

        private static void DrawEliteVymes(Color32[] p, int s, Color c)
        {
            DrawBruteVymes(p, s, c);
            FillDiamond(p, s, 16, 5, 4, new Color(1f, 0.75f, 1f, 1f));
            FillDiamond(p, s, 7, 18, 3, new Color(1f, 0.75f, 1f, 1f));
            FillDiamond(p, s, 25, 18, 3, new Color(1f, 0.75f, 1f, 1f));
        }

        private static void DrawBossVymes(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 2, 6, 8, 14, Shade(c, 0.25f));
            FillRect(p, s, 24, 6, 30, 14, Shade(c, 0.25f));
            DrawBlob(p, s, c, 14, 12, horned: true);
            FillRect(p, s, 8, 21, 24, 24, Shade(c, 0.35f));
            DrawEyes(p, s, 10, 13, 21, 13, new Color(1f, 0.75f, 0.25f, 1f));
            FillRect(p, s, 12, 19, 20, 20, new Color(0.1f, 0f, 0f, 1f));
        }

        private static void DrawShinyVymes(Color32[] p, int s, Color c)
        {
            FillDiamond(p, s, 16, 16, 15, new Color(0.45f, 0.35f, 0.08f, 1f));
            FillDiamond(p, s, 16, 16, 12, c);
            FillDiamond(p, s, 10, 9, 3, Color.white);
            FillDiamond(p, s, 23, 22, 3, Color.white);
            DrawEyes(p, s, 12, 15, 20, 15, new Color(0.15f, 0.08f, 0f, 1f));
        }

        private static void DrawTowerAltar(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 7, 23, 25, 28, Shade(c, 0.35f));
            FillRect(p, s, 10, 12, 22, 24, c);
            FillCircle(p, s, 16, 11, 7, Shade(c, 1.35f));
            FillCircle(p, s, 16, 11, 3, new Color(1f, 0.95f, 0.55f, 1f));
        }

        private static void DrawTowerArcher(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 10, 20, 22, 27, Shade(c, 0.4f));
            FillRect(p, s, 13, 9, 19, 22, c);
            FillDiamond(p, s, 16, 7, 6, new Color(1f, 0.95f, 0.45f, 1f));
            FillRect(p, s, 22, 8, 24, 24, new Color(0.6f, 0.42f, 0.18f, 1f));
        }

        private static void DrawTowerPoison(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 8, 22, 24, 27, Shade(c, 0.38f));
            FillRect(p, s, 11, 11, 21, 23, c);
            FillCircle(p, s, 16, 10, 6, new Color(0.45f, 1f, 0.3f, 1f));
            FillCircle(p, s, 11, 7, 2, new Color(0.7f, 1f, 0.5f, 0.8f));
            FillCircle(p, s, 23, 6, 2, new Color(0.7f, 1f, 0.5f, 0.8f));
        }

        private static void DrawTowerStorm(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 8, 22, 24, 27, Shade(c, 0.35f));
            FillRect(p, s, 12, 12, 20, 23, c);
            FillRect(p, s, 17, 3, 21, 13, new Color(0.7f, 0.9f, 1f, 1f));
            FillRect(p, s, 12, 12, 17, 17, new Color(0.7f, 0.9f, 1f, 1f));
            FillRect(p, s, 9, 17, 14, 27, new Color(0.7f, 0.9f, 1f, 1f));
        }

        private static void DrawTowerAura(Color32[] p, int s, Color c)
        {
            FillCircle(p, s, 16, 17, 13, Shade(c, 0.35f));
            FillCircle(p, s, 16, 17, 10, c);
            FillCircle(p, s, 16, 17, 5, new Color(1f, 1f, 1f, 0.9f));
            FillRect(p, s, 14, 5, 18, 29, Shade(c, 1.2f));
            FillRect(p, s, 5, 15, 27, 19, Shade(c, 1.2f));
        }

        private static void DrawChest(Color32[] p, int s, Color c)
        {
            FillRect(p, s, 6, 13, 26, 26, Shade(c, 0.38f));
            FillRect(p, s, 8, 11, 24, 18, c);
            FillRect(p, s, 14, 11, 18, 26, new Color(0.35f, 0.18f, 0.05f, 1f));
            FillRect(p, s, 13, 16, 19, 20, new Color(1f, 0.85f, 0.25f, 1f));
        }

        private static void DrawBolt(Color32[] p, int s, Color c)
        {
            FillDiamond(p, s, 16, 16, 13, Shade(c, 0.45f));
            FillRect(p, s, 5, 14, 26, 18, c);
            FillDiamond(p, s, 26, 16, 5, c);
            FillRect(p, s, 8, 15, 23, 17, Color.white);
        }

        private static void DrawOrb(Color32[] p, int s, Color c)
        {
            FillCircle(p, s, 16, 16, 11, Shade(c, 0.35f));
            FillCircle(p, s, 16, 16, 8, c);
            FillCircle(p, s, 13, 12, 3, Color.white);
        }

        private static void DrawNeedle(Color32[] p, int s, Color c)
        {
            FillDiamond(p, s, 18, 16, 12, Shade(c, 0.45f));
            FillRect(p, s, 5, 15, 24, 17, c);
            FillDiamond(p, s, 26, 16, 4, Color.white);
        }

        private static void DrawEnemyBolt(Color32[] p, int s, Color c)
        {
            FillCircle(p, s, 16, 16, 10, Shade(c, 0.4f));
            FillCircle(p, s, 16, 16, 7, c);
            FillDiamond(p, s, 16, 16, 4, new Color(0.1f, 0f, 0.05f, 1f));
        }

        private static void DrawBlob(Color32[] p, int s, Color c, int rx, int ry, bool horned)
        {
            Color outline = Shade(c, 0.25f);
            FillEllipse(p, s, 16, 17, rx + 2, ry + 2, outline);
            FillEllipse(p, s, 16, 17, rx, ry, c);
            if (!horned) return;
            FillRect(p, s, 6, 5, 11, 13, outline);
            FillRect(p, s, 21, 5, 26, 13, outline);
            FillRect(p, s, 8, 6, 11, 13, Shade(c, 1.25f));
            FillRect(p, s, 21, 6, 24, 13, Shade(c, 1.25f));
        }

        private static void DrawEyes(Color32[] p, int s, int x0, int y0, int x1, int y1, Color c)
        {
            FillRect(p, s, x0, y0, x0 + 2, y0 + 2, c);
            FillRect(p, s, x1, y1, x1 + 2, y1 + 2, c);
        }

        private static void FillCircle(Color32[] p, int s, int cx, int cy, int r, Color c)
        {
            int rr = r * r;
            for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                int dx = x - cx;
                int dy = y - cy;
                if (dx * dx + dy * dy <= rr) Set(p, s, x, y, c);
            }
        }

        private static void FillEllipse(Color32[] p, int s, int cx, int cy, int rx, int ry, Color c)
        {
            float rxs = rx * rx;
            float rys = ry * ry;
            for (int y = cy - ry; y <= cy + ry; y++)
            for (int x = cx - rx; x <= cx + rx; x++)
            {
                float dx = x - cx;
                float dy = y - cy;
                if (dx * dx / rxs + dy * dy / rys <= 1f) Set(p, s, x, y, c);
            }
        }

        private static void FillDiamond(Color32[] p, int s, int cx, int cy, int r, Color c)
        {
            for (int y = cy - r; y <= cy + r; y++)
            for (int x = cx - r; x <= cx + r; x++)
            {
                if (Mathf.Abs(x - cx) + Mathf.Abs(y - cy) <= r) Set(p, s, x, y, c);
            }
        }

        private static void FillRect(Color32[] p, int s, int x0, int y0, int x1, int y1, Color c)
        {
            for (int y = y0; y <= y1; y++)
            for (int x = x0; x <= x1; x++)
                Set(p, s, x, y, c);
        }

        private static void Set(Color32[] p, int s, int x, int y, Color c)
        {
            if (x < 0 || y < 0 || x >= s || y >= s) return;
            p[y * s + x] = c;
        }

        private static Color Shade(Color c, float multiplier)
        {
            return new Color(
                Mathf.Clamp01(c.r * multiplier),
                Mathf.Clamp01(c.g * multiplier),
                Mathf.Clamp01(c.b * multiplier),
                c.a);
        }
    }
}
