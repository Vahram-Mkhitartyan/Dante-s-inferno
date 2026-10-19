using DantesInferno.SinSystem;
using UnityEngine;

public static class LegacySinCompatibilityAdapter
{
    public static SinType ToSinType(DominantSin sin)
    {
        return (SinType)sin;
    }

    public static DominantSin ToDominantSin(SinType sin)
    {
        return (DominantSin)sin;
    }

    public static float CalculatePressure(
        float momentum,
        float maxMomentum,
        float stability,
        float maxStability)
    {
        float momentumPressure = maxMomentum <= 0f ? 0f : momentum / maxMomentum;
        float instabilityPressure = maxStability <= 0f ? 0f : 1f - stability / maxStability;
        return Mathf.Clamp01(Mathf.Max(momentumPressure, instabilityPressure));
    }
}
