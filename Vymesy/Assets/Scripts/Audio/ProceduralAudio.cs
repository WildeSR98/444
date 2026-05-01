using UnityEngine;

namespace Vymesy.Audio
{
    /// <summary>
    /// Generates simple procedural <see cref="AudioClip"/>s in code so the demo can play
    /// SFX without bundled audio assets. The synthesised clips intentionally lean into a
    /// dark-fantasy palette (low rumbles, dissonant intervals) and stay short.
    /// </summary>
    public static class ProceduralAudio
    {
        private const int SampleRate = 22050;

        public static AudioClip EnemyHit() => Build("vymesy_hit", 0.06f, (t, n) => Noise(t) * Decay(t, 0.06f) * 0.6f);

        public static AudioClip EnemyDeath() => Build("vymesy_death", 0.22f, (t, n) =>
        {
            float pitch = Mathf.Lerp(220f, 80f, t / 0.22f);
            return (Tone(pitch, n) + Noise(t) * 0.4f) * Decay(t, 0.22f) * 0.7f;
        });

        public static AudioClip BossDeath() => Build("vymesy_boss_death", 0.6f, (t, n) =>
        {
            float p1 = Mathf.Lerp(180f, 55f, t / 0.6f);
            float p2 = Mathf.Lerp(220f, 110f, t / 0.6f);
            return (Tone(p1, n) + 0.6f * Tone(p2, n) + Noise(t) * 0.5f) * Decay(t, 0.6f) * 0.85f;
        });

        public static AudioClip PlayerHit() => Build("player_hit", 0.18f, (t, n) =>
        {
            float pitch = Mathf.Lerp(440f, 160f, t / 0.18f);
            return (Tone(pitch, n) * 0.6f + Noise(t) * 0.4f) * Decay(t, 0.18f) * 0.55f;
        });

        public static AudioClip CritImpact() => Build("crit", 0.15f, (t, n) =>
        {
            float pitch = Mathf.Lerp(900f, 420f, t / 0.15f);
            return Tone(pitch, n) * Decay(t, 0.15f) * 0.6f;
        });

        public static AudioClip LevelUp() => Build("levelup", 0.45f, (t, n) =>
        {
            // Arpeggio: 440 → 550 → 660 → 880
            float[] steps = { 440f, 550f, 660f, 880f };
            int idx = Mathf.Clamp(Mathf.FloorToInt(t / 0.45f * steps.Length), 0, steps.Length - 1);
            return Tone(steps[idx], n) * Decay(t % 0.115f, 0.115f) * 0.5f;
        });

        public static AudioClip Pickup() => Build("pickup", 0.10f, (t, n) =>
        {
            float pitch = Mathf.Lerp(660f, 1100f, t / 0.10f);
            return Tone(pitch, n) * Decay(t, 0.10f) * 0.4f;
        });

        public static AudioClip BossSummon() => Build("boss_summon", 0.85f, (t, n) =>
        {
            float p1 = 92f;
            float p2 = 138f * (1f + 0.05f * Mathf.Sin(t * 6f));
            return (Tone(p1, n) + Tone(p2, n) * 0.55f + Noise(t) * 0.2f) * Decay(t, 0.85f) * 0.7f;
        });

        public static AudioClip RunStart() => Build("run_start", 0.5f, (t, n) =>
        {
            float pitch = Mathf.Lerp(110f, 440f, t / 0.5f);
            return Tone(pitch, n) * Decay(t, 0.5f) * 0.4f;
        });

        public static AudioClip RunEnd() => Build("run_end", 0.7f, (t, n) =>
        {
            float pitch = Mathf.Lerp(330f, 80f, t / 0.7f);
            return (Tone(pitch, n) + Noise(t) * 0.3f) * Decay(t, 0.7f) * 0.55f;
        });

        public static AudioClip MenuPad()
        {
            const float duration = 4f;
            return Build("menu_pad", duration, (t, n) =>
            {
                float a = Tone(110f, n) * 0.35f;
                float b = Tone(165f, n) * 0.25f;
                float c = Tone(220f, n) * 0.20f * (0.5f + 0.5f * Mathf.Sin(t * 0.5f));
                return (a + b + c) * (0.5f + 0.5f * Mathf.Sin(t * 0.7f)) * 0.6f;
            }, loop: true);
        }

        public static AudioClip RunPad()
        {
            const float duration = 6f;
            return Build("run_pad", duration, (t, n) =>
            {
                float bass = Tone(82f, n) * 0.4f;
                float mid = Tone(123f, n) * 0.2f * Mathf.Abs(Mathf.Sin(t * 1.1f));
                float wash = Tone(247f, n) * 0.1f * Mathf.Abs(Mathf.Sin(t * 0.7f));
                return (bass + mid + wash) * 0.6f;
            }, loop: true);
        }

        private static AudioClip Build(string name, float seconds, System.Func<float, int, float> sampleFunc, bool loop = false)
        {
            int sampleCount = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[sampleCount];
            for (int n = 0; n < sampleCount; n++)
            {
                float t = n / (float)SampleRate;
                data[n] = Mathf.Clamp(sampleFunc(t, n), -1f, 1f);
            }
            var clip = AudioClip.Create(name, sampleCount, 1, SampleRate, false);
            clip.SetData(data, 0);
            // Loops are governed by AudioSource.loop, but we record intent via name suffix.
            return clip;
        }

        private static float Tone(float frequency, int sampleIndex)
            => Mathf.Sin(2f * Mathf.PI * frequency * (sampleIndex / (float)SampleRate));

        private static float Noise(float t) => (Mathf.PerlinNoise(t * 911f, 0.3f) - 0.5f) * 2f;

        private static float Decay(float t, float lifetime)
        {
            float k = Mathf.Clamp01(1f - t / Mathf.Max(0.0001f, lifetime));
            return k * k;
        }
    }
}
