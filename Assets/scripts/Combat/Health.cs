using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour
{
    public System.Action OnDamaged;
    public System.Action OnDeath;

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

    public void TakeDamage(int amount)
    {
        if (isDying) return;

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


    
    IEnumerator BlinkRoutine()
    {
        sr.enabled = false;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
        sr.enabled = true;
        yield return new WaitForSecondsRealtime(blinkTime * 0.5f);
    }
}
