using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    public bool IsKnockedBack { get; private set; }
    public float knockbackMultiplier = 1f;
    public float knockbackDuration = 0.15f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 force)
    {
        if (rb == null) return;
        StopAllCoroutines();
        Vector2 final = force * Mathf.Max(0f, knockbackMultiplier);
        StartCoroutine(KnockRoutine(final));
    }

    IEnumerator KnockRoutine(Vector2 force)
    {
        IsKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(knockbackDuration);
        IsKnockedBack = false;
    }
}
