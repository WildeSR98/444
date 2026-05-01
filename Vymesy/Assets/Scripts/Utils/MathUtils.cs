using UnityEngine;

namespace Vymesy.Utils
{
    public static class MathUtils
    {
        public static Vector2 RandomOnUnitCircle()
        {
            float a = Random.value * Mathf.PI * 2f;
            return new Vector2(Mathf.Cos(a), Mathf.Sin(a));
        }

        public static Vector2 RandomInAnnulus(float minRadius, float maxRadius)
        {
            return RandomOnUnitCircle() * Random.Range(minRadius, maxRadius);
        }

        public static bool RollChance(float chance01) => Random.value < chance01;

        public static int WeightedPick(int[] weights)
        {
            if (weights == null || weights.Length == 0) return -1;
            int total = 0;
            for (int i = 0; i < weights.Length; i++) total += Mathf.Max(0, weights[i]);
            if (total <= 0) return -1;
            int roll = Random.Range(0, total);
            int acc = 0;
            for (int i = 0; i < weights.Length; i++)
            {
                acc += Mathf.Max(0, weights[i]);
                if (roll < acc) return i;
            }
            return weights.Length - 1;
        }
    }
}
