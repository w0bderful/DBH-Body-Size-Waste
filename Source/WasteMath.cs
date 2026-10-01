using System;

namespace DBHBodySizeWaste
{
    public static class WasteMath
    {
        public static float Valid(float value, float fallback, float min, float max)
        {
            if (float.IsNaN(value) || float.IsInfinity(value)) return fallback;
            return Math.Max(min, Math.Min(max, value));
        }

        public static float Factor(float bodySize, float multiplier, float exponent)
        {
            bodySize = Valid(bodySize, 1f, 0f, float.MaxValue);
            multiplier = Valid(multiplier, 1f, 0f, 10f);
            exponent = Valid(exponent, 1f, 0f, 2f);
            // Keep pathological modded body sizes from overflowing the sewage simulation.
            return (float)Math.Min(10000.0, multiplier * Math.Pow(bodySize, exponent));
        }
    }
}
