using System;

public static class EnemyEvents
{
    public static event Action<EnemyType> OnEnemyKilled;

    public static void RaiseEnemyKilled(EnemyType type)
    {
        OnEnemyKilled?.Invoke(type);
    }
}
