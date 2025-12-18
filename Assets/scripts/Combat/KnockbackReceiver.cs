using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    public bool IsKnockedBack { get; private set; }
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 dir, float force)
    {
        StopAllCoroutines();
        StartCoroutine(KnockRoutine(dir.normalized * force));
    }

    IEnumerator KnockRoutine(Vector2 force)
    {
        IsKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        IsKnockedBack = false;
    }
}
