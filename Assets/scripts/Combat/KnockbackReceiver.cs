using UnityEngine;
using System.Collections;

public class KnockbackReceiver : MonoBehaviour
{
    public bool IsKnockedBack { get; private set; }
    public float transformKnockbackScale = 1f;
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
    }

    public void ApplyKnockback(Vector2 dir, float force)
    {
        StopAllCoroutines();
        Vector2 final = dir.normalized * force;

        if (rb != null)
        {
            StartCoroutine(KnockRoutine(final));
            return;
        }

        StartCoroutine(KnockTransformRoutine(final));
    }

    IEnumerator KnockRoutine(Vector2 force)
    {
        IsKnockedBack = true;
        rb.linearVelocity = Vector2.zero;
        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(0.15f);
        IsKnockedBack = false;
    }

    IEnumerator KnockTransformRoutine(Vector2 force)
    {
        IsKnockedBack = true;
        float timer = 0f;
        float duration = 0.15f;

        while (timer < duration)
        {
            transform.position += (Vector3)(force * transformKnockbackScale * Time.deltaTime);
            timer += Time.deltaTime;
            yield return null;
        }

        IsKnockedBack = false;
    }
}
