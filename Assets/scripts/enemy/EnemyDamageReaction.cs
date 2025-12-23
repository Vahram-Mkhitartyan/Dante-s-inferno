using UnityEngine;
using System.Collections;

public class EnemyDamageReaction : MonoBehaviour
{
    [SerializeField] private float destroyDelay = 0.08f;

    private Health health;

    void Awake()
    {
        health = GetComponent<Health>();
        health.OnDeath += OnDeath;
    }

    void OnDeath()
    {
        StartCoroutine(DieRoutine());
    }

    IEnumerator DieRoutine()
    {
        // Disable enemy behavior
        var move = GetComponentInChildren<EnemyMovement>();
        if (move) move.enabled = false;

        var atk = GetComponentInChildren<EnemyAttack>();
        if (atk) atk.enabled = false;

        // Award sin
        var state = GetComponentInChildren<EnemyState>();
        if (SinManager.Instance && state != null)
        {
            int add = state.IsHostile ? 0 : 3;
            SinManager.Instance.AddSin(add);
        }

        yield return new WaitForSecondsRealtime(destroyDelay);
        Destroy(gameObject);
    }
}
