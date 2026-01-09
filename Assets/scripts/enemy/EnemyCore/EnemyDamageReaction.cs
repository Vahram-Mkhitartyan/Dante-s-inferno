using UnityEngine;

public class EnemyDamageReaction : MonoBehaviour
{
    private EnemyState state;
    private Health health;
    private EnemyIdentity identity;

    void Awake()
    {
        state = GetComponent<EnemyState>();
        health = GetComponent<Health>();
        identity = GetComponent<EnemyIdentity>();

        if (health != null)
            health.OnDeath += HandleDeath;
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        if (state != null)
        {
            if (!state.IsHostile && SinManager.Instance != null)
            {
                SinManager.Instance.AddSin(1);
            }

            state.SetHostile(false);
        }

        if (identity != null)
            EnemyEvents.RaiseEnemyKilled(identity.type);
    }
}
