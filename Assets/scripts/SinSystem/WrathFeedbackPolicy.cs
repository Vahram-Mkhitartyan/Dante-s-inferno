using System;

namespace DantesInferno.SinSystem
{
    public static class WrathFeedbackPolicy
    {
        public const float StirringThreshold = 0.2f;
        public const float SurgingThreshold = 0.5f;
        public const float OverwhelmingThreshold = 0.8f;

        public static WrathFeedbackLevel GetLevel(float pressure)
        {
            float normalizedPressure = Clamp01(pressure);
            if (normalizedPressure >= OverwhelmingThreshold)
                return WrathFeedbackLevel.Overwhelming;
            if (normalizedPressure >= SurgingThreshold)
                return WrathFeedbackLevel.Surging;
            if (normalizedPressure >= StirringThreshold)
                return WrathFeedbackLevel.Stirring;

            return WrathFeedbackLevel.Quiet;
        }

        public static float GetVisualIntensity(float pressure, bool isDominant)
        {
            float normalizedPressure = Clamp01(pressure);
            float dominanceScale = isDominant ? 1f : 0.45f;
            return Clamp01(normalizedPressure * dominanceScale);
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
