using System;

public static class PlayerActionEvents
{
    public delegate int DamageModifier(int baseDamage);

    public static event Action<AttackType> OnAttackStarted;
    public static event Action<AttackType> OnAttackHit;
    public static event Action OnDamageBlocked;
    public static event Action OnDamageTaken;
    public static event Action OnPlayerDied;
    public static event Action OnNonHostileEnemyKilled;
    public static event DamageModifier OnModifyOutgoingDamage;

    public static void RaiseAttackStarted(AttackType attack)
    {
        OnAttackStarted?.Invoke(attack);
    }

    public static void RaiseAttackHit(AttackType attack)
    {
        OnAttackHit?.Invoke(attack);
    }

    public static void RaiseDamageBlocked()
    {
        OnDamageBlocked?.Invoke();
    }

    public static void RaiseDamageTaken()
    {
        OnDamageTaken?.Invoke();
    }

    public static void RaisePlayerDied()
    {
        OnPlayerDied?.Invoke();
    }

    public static void RaiseNonHostileEnemyKilled()
    {
        OnNonHostileEnemyKilled?.Invoke();
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
}
