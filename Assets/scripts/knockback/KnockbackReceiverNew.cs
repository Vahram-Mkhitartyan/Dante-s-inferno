using UnityEngine;
using System.Collections;

public class KnockbackReceiverNew : MonoBehaviour
{
    [Header("Tuning")]
    public float coefficient = 1f;
    public float duration = 0.15f;
    public bool clearVelocity = true;
    public bool ignoreWhenDefending = true;

    public bool IsKnockedBack { get; private set; }

    private Rigidbody2D rb;
    private PlayerController player;

    void Awake()
    {
        rb = GetComponentInParent<Rigidbody2D>();
        player = GetComponentInParent<PlayerController>();
    }

    public void Apply(Vector2 direction, float baseForce)
    {
        if (rb == null) return;
        if (ignoreWhenDefending && player != null && player.IsDefending) return;

        float scale = Mathf.Max(0f, coefficient);
        Vector2 final = direction.normalized * baseForce * scale;

        StopAllCoroutines();
        StartCoroutine(KnockRoutine(final));
    }

    IEnumerator KnockRoutine(Vector2 force)
    {
        IsKnockedBack = true;

        if (clearVelocity)
            rb.linearVelocity = Vector2.zero;

        rb.AddForce(force, ForceMode2D.Impulse);
        yield return new WaitForSeconds(duration);
        IsKnockedBack = false;
    }
}
