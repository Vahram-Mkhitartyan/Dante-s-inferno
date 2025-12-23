using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public System.Action OnDamaged;
    public System.Action OnDeath;
    public System.Action OnBlocked;

    [Header("Health")]
    public int maxHealth = 3;

    [Header("Hit Feedback")]
    public float blinkTime = 0.08f;

    private int currentHealth;
    private bool isDying;
    
    private SpriteRenderer sr;
    public bool IsDead => isDying;


    void Awake()
    {
        currentHealth = maxHealth;
        sr = GetComponentInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount, Vector2 attackerPosition)
    {
        if (isDying) return;

        PlayerController pc = GetComponent<PlayerController>();

        // --- DEFENSE CHECK ---
        if (pc && pc.IsDefending)
        {
            Vector2 toAttacker = (attackerPosition - (Vector2)transform.position).normalized;

            // FacingDirectionVector points FORWARD
            float dot = Vector2.Dot(pc.FacingDirectionVector, toAttacker);

            // dot > 0 → attacker in front
            if (dot > 0f)
            {
                OnBlocked?.Invoke();
                return;
            }
        }

        // --- APPLY DAMAGE ---
        currentHealth -= amount;

        if (currentHealth > 0)
        {
            OnDamaged?.Invoke();
        }
        else
        {
            isDying = true;
            OnDeath?.Invoke();
        }
    }


    public void ResetHealth()
    {
        currentHealth = maxHealth;
        isDying = false;
    }



    
    IEnumerator BlinkRoutine()
    {
        sr.enabled = false;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
        sr.enabled = true;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
    }
}
