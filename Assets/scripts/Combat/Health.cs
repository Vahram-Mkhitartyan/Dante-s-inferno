using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;

    [Header("Hit/Death Feedback")]
    public float blinkTime = 0.08f;
    public float deathDelay = 0.08f;

    private int currentHealth;
    private bool isDying = false;

    private SpriteRenderer sr;

    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount)
    {
        if (isDying) return;

        currentHealth -= amount;
        Debug.Log($"{gameObject.name} took damage. HP: {currentHealth}");

        if (sr && blinkTime > 0f)
            StartCoroutine(BlinkRoutine());

        if (currentHealth <= 0)
        {
            TryAwardSin();     // ← ADD HERE
            StartCoroutine(DieRoutine());
        }

    }

    void TryAwardSin()
    {
        if (!SinManager.Instance)
        {
            Debug.LogWarning($"[SIN AWARD] FAILED for {gameObject.name}. SinManager missing.");
            return;
        }

        var state = GetComponent<EnemyState>();
        if (!state)
        {
            Debug.LogWarning($"[SIN AWARD] FAILED for {gameObject.name}. No EnemyState.");
            return;
        }

        // Only neutral kills give sin
        if (!state.IsHostile)
        {
            SinManager.Instance.AddSin(1);
        }
    }



    IEnumerator BlinkRoutine()
    {
        sr.enabled = false;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
        sr.enabled = true;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
    }

    IEnumerator DieRoutine()
    {
        isDying = true;

        // Disable behavior scripts (supports child components too)
        var move = GetComponentInChildren<EnemyMovement>();
        if (move) move.enabled = false;

        var atk = GetComponentInChildren<EnemyAttack>();
        if (atk) atk.enabled = false;

        // ✅ Award sin BEFORE destroy
        var state = GetComponentInChildren<EnemyState>();
        if (SinManager.Instance && state != null)
        {
            int add = state.IsHostile ? 0 : 3; // hostile kill smaller sin, neutral bigger
            SinManager.Instance.AddSin(add);
            Debug.Log($"[SIN AWARD] {name} IsHostile={state.IsHostile} +{add}");
        }
        else
        {
            Debug.LogWarning($"[SIN AWARD] FAILED for {name}. SinManager? {(SinManager.Instance != null)} EnemyState? {(state != null)}");
        }

        yield return new WaitForSecondsRealtime(deathDelay);
        Destroy(gameObject);
    }
}
