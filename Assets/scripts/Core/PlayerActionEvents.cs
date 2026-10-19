using System;
using DantesInferno.SinSystem;

public static class PlayerActionEvents
{
    public delegate int DamageModifier(int baseDamage);

    public static event Action<AttackType> OnAttackStarted;
    public static event Action<AttackType> OnAttackHit;
    public static event Action OnDamageBlocked;
    public static event Action OnDamageTaken;
    public static event Action OnPlayerDied;
    public static event Action OnNonHostileEnemyKilled;
    public static event Action<SinActionRecord> OnActionRecorded;
    public static event DamageModifier OnModifyOutgoingDamage;

    private static string runId = Guid.NewGuid().ToString("N");
    private static long nextSequence = 1;

    public static string RunId => runId;
    public static long NextSequence => nextSequence;

    public static void RaiseAttackStarted(AttackType attack)
    {
        OnAttackStarted?.Invoke(attack);
        RaiseContextualAction(SinActionKind.AttackStarted, GetCurrentBehaviorPattern());
    }

    public static void RaiseAttackHit(AttackType attack)
    {
        OnAttackHit?.Invoke(attack);
        RaiseContextualAction(SinActionKind.AttackHit, GetCurrentBehaviorPattern());
    }

    public static void RaiseDamageBlocked()
    {
        OnDamageBlocked?.Invoke();
        RaiseContextualAction(SinActionKind.DamageBlocked);
    }

    public static void RaiseDamageTaken()
    {
        OnDamageTaken?.Invoke();
        RaiseContextualAction(SinActionKind.DamageTaken);
    }

    public static void RaisePlayerDied()
    {
        OnPlayerDied?.Invoke();
        RaiseContextualAction(SinActionKind.PlayerDied);
    }

    public static void RaiseNonHostileEnemyKilled()
    {
        OnNonHostileEnemyKilled?.Invoke();
        RaiseContextualAction(SinActionKind.NonHostileEnemyKilled);
    }

    public static void RaiseRewardCollected(
        float rewardValue,
        string encounterId,
        float availableAlternatives,
        float danger)
    {
        RaiseContextualAction(
            SinActionKind.RewardCollected,
            encounterId: encounterId,
            rewardValue: rewardValue,
            availableAlternatives: availableAlternatives,
            danger: danger);
    }

    public static void RaiseRewardRejected(float rewardValue, string encounterId, float danger)
    {
        RaiseContextualAction(
            SinActionKind.RewardRejected,
            encounterId: encounterId,
            rewardValue: rewardValue,
            danger: danger);
    }

    public static void RaiseRewardInvested(float rewardValue, string encounterId, float danger)
    {
        RaiseContextualAction(
            SinActionKind.RewardInvested,
            encounterId: encounterId,
            rewardValue: rewardValue,
            danger: danger);
    }

    public static int ModifyOutgoingDamage(int baseDamage)
    {
        int modified = baseDamage;
        if (OnModifyOutgoingDamage == null)
            return modified;

        foreach (DamageModifier modifier in OnModifyOutgoingDamage.GetInvocationList())
            modified = Math.Max(1, modifier(modified));

        return modified;
    }

    public static void ConfigureRun(string restoredRunId, long restoredNextSequence)
    {
        if (string.IsNullOrWhiteSpace(restoredRunId))
            throw new ArgumentException("A run identifier is required.", nameof(restoredRunId));

        runId = restoredRunId;
        nextSequence = Math.Max(1, restoredNextSequence);
    }

    public static void ResetRuntimeState()
    {
        OnAttackStarted = null;
        OnAttackHit = null;
        OnDamageBlocked = null;
        OnDamageTaken = null;
        OnPlayerDied = null;
        OnNonHostileEnemyKilled = null;
        OnActionRecorded = null;
        OnModifyOutgoingDamage = null;
        runId = Guid.NewGuid().ToString("N");
        nextSequence = 1;
    }

    private static void RaiseContextualAction(
        SinActionKind kind,
        SinBehaviorPattern behaviorPattern = SinBehaviorPattern.Balanced,
        string encounterId = "",
        float rewardValue = 0f,
        float availableAlternatives = 0f,
        float danger = 0f)
    {
        var action = new SinActionRecord(
            new SinEventId(runId, nextSequence++),
            kind,
            DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() / 1000d,
            encounterId,
            behaviorPattern,
            rewardValue: rewardValue,
            availableAlternatives: availableAlternatives,
            danger: danger);

        OnActionRecorded?.Invoke(action);
    }

    private static SinBehaviorPattern GetCurrentBehaviorPattern()
    {
        if (PlayerBehaviorTracker.Instance == null)
            return SinBehaviorPattern.Balanced;

        switch (PlayerBehaviorTracker.Instance.CurrentPattern)
        {
            case PlayerBehaviorPattern.RepeatedAttack:
                return SinBehaviorPattern.Repeated;
            case PlayerBehaviorPattern.AlternatingAttack:
                return SinBehaviorPattern.Alternating;
            case PlayerBehaviorPattern.Overcommitted:
                return SinBehaviorPattern.Overcommitted;
            default:
                return SinBehaviorPattern.Balanced;
        }
    }
}
