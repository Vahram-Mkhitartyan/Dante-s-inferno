using UnityEngine;

public class EnemyDamageReaction : MonoBehaviour
{
    private EnemyState state;

    void Awake()
    {
        state = GetComponent<EnemyState>();
    }

    public void OnDeath()
    {
        if (state != null)
            state.SetHostile(false);
    }
}
