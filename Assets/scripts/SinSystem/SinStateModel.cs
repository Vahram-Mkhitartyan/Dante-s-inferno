using System;

namespace DantesInferno.SinSystem
{
    public sealed class SinStateModel
    {
        private const int SinCount = 7;

        private readonly SinStateRules rules;
        private readonly float[] influenceWeights = new float[SinCount];
        private readonly float[] pressures = new float[SinCount];
        private readonly float[] mastery = new float[SinCount];

        private SinType dominanceCandidate;
        private float dominanceCandidateSeconds;

        public SinType OriginSin { get; }
        public SinType CurrentDominantSin { get; private set; }
        public float OverallPressure => CalculateOverallPressure();
        public SinPressureState OverallState => CalculateOverallState(OverallPressure);

        public SinStateModel(SinType originSin, SinStateRules rules = null)
        {
            ValidateSin(originSin);

            this.rules = rules ?? SinStateRules.Default;
            OriginSin = originSin;
            CurrentDominantSin = originSin;
            dominanceCandidate = originSin;

            for (int index = 0; index < SinCount; index++)
            {
                influenceWeights[index] = index == (int)originSin
                    ? this.rules.OriginInfluenceWeight
                    : this.rules.SecondaryInfluenceWeight;
            }
        }

        public float GetInfluenceWeight(SinType sin)
        {
            return influenceWeights[GetIndex(sin)];
        }

        public float GetComposition(SinType sin)
        {
            float total = GetTotalInfluenceWeight();
            return influenceWeights[GetIndex(sin)] / total;
        }

        public float GetPressure(SinType sin)
        {
            return pressures[GetIndex(sin)];
        }

        public float GetMastery(SinType sin)
        {
            return mastery[GetIndex(sin)];
        }

        public void AddInfluence(SinType sin, float amount)
        {
            int index = GetIndex(sin);
            influenceWeights[index] = Math.Max(rules.MinimumInfluenceWeight, influenceWeights[index] + amount);
        }

        public void SetPressure(SinType sin, float value)
        {
            pressures[GetIndex(sin)] = Clamp01(value);
        }

        public void AddPressure(SinType sin, float amount)
        {
            SetPressure(sin, GetPressure(sin) + amount);
        }

        public void SetMastery(SinType sin, float value)
        {
            mastery[GetIndex(sin)] = Clamp01(value);
        }

        public void AddMastery(SinType sin, float amount)
        {
            SetMastery(sin, GetMastery(sin) + amount);
        }

        public void AdvanceDominance(float deltaSeconds)
        {
            if (deltaSeconds < 0f)
                throw new ArgumentOutOfRangeException(nameof(deltaSeconds));

            SinType strongest = FindStrongestSin();
            if (strongest == CurrentDominantSin || !ExceedsDominanceMargin(strongest))
            {
                dominanceCandidate = CurrentDominantSin;
                dominanceCandidateSeconds = 0f;
                return;
            }

            if (strongest != dominanceCandidate)
            {
                dominanceCandidate = strongest;
                dominanceCandidateSeconds = 0f;
            }

            dominanceCandidateSeconds += deltaSeconds;
            if (dominanceCandidateSeconds < rules.DominanceSustainSeconds)
                return;

            CurrentDominantSin = dominanceCandidate;
            dominanceCandidateSeconds = 0f;
        }

        public SinStateSnapshot CreateSnapshot()
        {
            var axes = new SinAxisSnapshot[SinCount];
            for (int index = 0; index < SinCount; index++)
            {
                SinType sin = (SinType)index;
                axes[index] = new SinAxisSnapshot(
                    sin,
                    influenceWeights[index],
                    GetComposition(sin),
                    pressures[index],
                    mastery[index]);
            }

            return new SinStateSnapshot(
                OriginSin,
                CurrentDominantSin,
                OverallPressure,
                OverallState,
                axes);
        }

        private SinType FindStrongestSin()
        {
            SinType strongest = CurrentDominantSin;
            float strongestScore = GetDominanceScore(strongest);

            for (int index = 0; index < SinCount; index++)
            {
                SinType candidate = (SinType)index;
                float candidateScore = GetDominanceScore(candidate);
                if (candidateScore > strongestScore)
                {
                    strongest = candidate;
                    strongestScore = candidateScore;
                }
            }

            return strongest;
        }

        private bool ExceedsDominanceMargin(SinType challenger)
        {
            float challengerScore = GetDominanceScore(challenger);
            float currentScore = GetDominanceScore(CurrentDominantSin);
            return challengerScore >= currentScore + rules.DominanceMargin;
        }

        private float GetDominanceScore(SinType sin)
        {
            float pressureWeight = rules.PressureDominanceWeight;
            float compositionWeight = 1f - pressureWeight;
            return GetComposition(sin) * compositionWeight + GetPressure(sin) * pressureWeight;
        }

        private float CalculateOverallPressure()
        {
            switch (rules.PressureAggregation)
            {
                case OverallPressureAggregation.Maximum:
                    return CalculateMaximumPressure();
                case OverallPressureAggregation.Average:
                    return CalculateAveragePressure();
                case OverallPressureAggregation.CombinedRisk:
                    return CalculateCombinedRisk();
                default:
                    throw new InvalidOperationException($"Unsupported pressure aggregation: {rules.PressureAggregation}");
            }
        }

        private float CalculateMaximumPressure()
        {
            float maximum = 0f;
            for (int index = 0; index < SinCount; index++)
                maximum = Math.Max(maximum, pressures[index]);

            return maximum;
        }

        private float CalculateAveragePressure()
        {
            float total = 0f;
            for (int index = 0; index < SinCount; index++)
                total += pressures[index];

            return total / SinCount;
        }

        private float CalculateCombinedRisk()
        {
            float remainingSafety = 1f;
            for (int index = 0; index < SinCount; index++)
                remainingSafety *= 1f - pressures[index];

            return Clamp01(1f - remainingSafety);
        }

        private SinPressureState CalculateOverallState(float pressure)
        {
            if (pressure >= rules.CriticalThreshold)
                return SinPressureState.Critical;
            if (pressure >= rules.UnstableThreshold)
                return SinPressureState.Unstable;
            if (pressure >= rules.RisingThreshold)
                return SinPressureState.Rising;

            return SinPressureState.Controlled;
        }

        private float GetTotalInfluenceWeight()
        {
            float total = 0f;
            for (int index = 0; index < SinCount; index++)
                total += influenceWeights[index];

            return total;
        }

        private static int GetIndex(SinType sin)
        {
            ValidateSin(sin);
            return (int)sin;
        }

        private static void ValidateSin(SinType sin)
        {
            int index = (int)sin;
            if (index < 0 || index >= SinCount)
                throw new ArgumentOutOfRangeException(nameof(sin));
        }

        private static float Clamp01(float value)
        {
            return Math.Max(0f, Math.Min(1f, value));
        }
    }
}
