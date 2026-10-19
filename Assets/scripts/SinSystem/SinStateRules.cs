using System;

namespace DantesInferno.SinSystem
{
    public sealed class SinStateRules
    {
        public const float DefaultOriginInfluence = 34f;
        public const float DefaultSecondaryInfluence = 11f;

        public float MinimumInfluenceWeight { get; }
        public float OriginInfluenceWeight { get; }
        public float SecondaryInfluenceWeight { get; }
        public float PressureDominanceWeight { get; }
        public float DominanceMargin { get; }
        public float DominanceSustainSeconds { get; }
        public float RisingThreshold { get; }
        public float UnstableThreshold { get; }
        public float CriticalThreshold { get; }
        public OverallPressureAggregation PressureAggregation { get; }

        public static SinStateRules Default { get; } = new SinStateRules();

        public SinStateRules(
            float minimumInfluenceWeight = 0.01f,
            float originInfluenceWeight = DefaultOriginInfluence,
            float secondaryInfluenceWeight = DefaultSecondaryInfluence,
            float pressureDominanceWeight = 0.5f,
            float dominanceMargin = 0.05f,
            float dominanceSustainSeconds = 2f,
            float risingThreshold = 0.35f,
            float unstableThreshold = 0.7f,
            float criticalThreshold = 0.9f,
            OverallPressureAggregation pressureAggregation = OverallPressureAggregation.CombinedRisk)
        {
            MinimumInfluenceWeight = RequirePositive(minimumInfluenceWeight, nameof(minimumInfluenceWeight));
            OriginInfluenceWeight = RequireAtLeast(originInfluenceWeight, MinimumInfluenceWeight, nameof(originInfluenceWeight));
            SecondaryInfluenceWeight = RequireAtLeast(secondaryInfluenceWeight, MinimumInfluenceWeight, nameof(secondaryInfluenceWeight));
            PressureDominanceWeight = RequireRange(pressureDominanceWeight, 0f, 1f, nameof(pressureDominanceWeight));
            DominanceMargin = RequireRange(dominanceMargin, 0f, 1f, nameof(dominanceMargin));
            DominanceSustainSeconds = RequireAtLeast(dominanceSustainSeconds, 0f, nameof(dominanceSustainSeconds));
            RisingThreshold = RequireRange(risingThreshold, 0f, 1f, nameof(risingThreshold));
            UnstableThreshold = RequireRange(unstableThreshold, RisingThreshold, 1f, nameof(unstableThreshold));
            CriticalThreshold = RequireRange(criticalThreshold, UnstableThreshold, 1f, nameof(criticalThreshold));
            PressureAggregation = pressureAggregation;
        }

        private static float RequirePositive(float value, string parameterName)
        {
            if (value <= 0f)
                throw new ArgumentOutOfRangeException(parameterName, "Value must be greater than zero.");

            return value;
        }

        private static float RequireAtLeast(float value, float minimum, string parameterName)
        {
            if (value < minimum)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be at least {minimum}.");

            return value;
        }

        private static float RequireRange(float value, float minimum, float maximum, string parameterName)
        {
            if (value < minimum || value > maximum)
                throw new ArgumentOutOfRangeException(parameterName, $"Value must be between {minimum} and {maximum}.");

            return value;
        }
    }
}
