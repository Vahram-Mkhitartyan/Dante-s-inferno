using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(menuName = "Dantes Inferno/Sin Definition")]
public class SinDefinition : ScriptableObject
{
    public DominantSin sin = DominantSin.Wrath;

    [Header("Momentum")]
    public float defaultAttackMomentum = 2.5f;
    [FormerlySerializedAs("swordMomentum")] public float topDownMomentum = 4f;
    [FormerlySerializedAs("kickMomentum")] public float upwardMomentum = 3f;
    [FormerlySerializedAs("spinMomentum")] public float pierceMomentum = 4f;
    public float hitMomentumMultiplier = 0.5f;
    public float nonHostileKillMomentum = 1f;

    [Header("Stability")]
    public float stableAttackCost = 2f;
    public float repeatedPatternCost = 8f;
    public float overcommitCost = 6f;
    public float blockedDamageRecovery = 4f;
    public float damageTakenCost = 12f;

    [Header("Power")]
    public float maxPowerBonus = 0.75f;

    [Header("Collapse")]
    public float collapseMomentumLoss = 25f;
    public float collapseStabilityRecovery = 35f;

    public float GetAttackMomentum(AttackType attack)
    {
        switch (attack)
        {
            case AttackType.TopDownSwing:
                return topDownMomentum;
            case AttackType.UpwardLauncher:
                return upwardMomentum;
            case AttackType.ForwardPierce:
                return pierceMomentum;
            default:
                return defaultAttackMomentum;
        }
    }

    public float GetPatternStabilityCost(PlayerBehaviorPattern pattern)
    {
        switch (pattern)
        {
            case PlayerBehaviorPattern.RepeatedAttack:
            case PlayerBehaviorPattern.AlternatingAttack:
                return repeatedPatternCost;
            case PlayerBehaviorPattern.Overcommitted:
                return overcommitCost;
            default:
                return stableAttackCost;
        }
    }
}
