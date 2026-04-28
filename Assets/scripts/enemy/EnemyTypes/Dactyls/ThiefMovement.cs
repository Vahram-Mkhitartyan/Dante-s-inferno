using UnityEngine;

public class ThiefMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 0.8f;
    public float fleeSpeedMultiplier = 1.8f;
    public float stopDistance = 1.0f;

    private float fleeUntilTime = -1f;
    private Rigidbody2D rb;
    private KnockbackReceiverNew knockback;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockbackReceiverNew>();
        if (knockback == null)
            knockback = gameObject.AddComponent<KnockbackReceiverNew>();
    }

    public void TriggerFlee(float duration)
    {
        fleeUntilTime = Time.time + duration;
    }

    public void TickMovement(Transform target)
    {
        if (target == null) return;
        if (knockback != null && knockback.IsKnockedBack) return;

        // Flee overrides normal movement for a short window.
        if (Time.time < fleeUntilTime)
        {
            Vector2 away = ((Vector2)transform.position - (Vector2)target.position).normalized;
            Move(away * moveSpeed * fleeSpeedMultiplier * Time.deltaTime);
            return;
        }

        // Normal chase until within stop distance.
        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= stopDistance) return;

        Vector2 dir = ((Vector2)target.position - (Vector2)transform.position).normalized;
        Move(dir * moveSpeed * Time.deltaTime);
    }

    void Move(Vector2 delta)
    {
        if (rb == null || rb.bodyType == RigidbodyType2D.Static)
        {
            transform.position += (Vector3)delta;
            return;
        }

        rb.MovePosition(rb.position + delta);
    }
}
