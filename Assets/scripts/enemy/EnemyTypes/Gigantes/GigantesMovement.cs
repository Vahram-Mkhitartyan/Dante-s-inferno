using UnityEngine;

public class GigantesMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 1.1f;
    public float stopDistance = 1.6f;

    private GigantesAttack attack;
    private Rigidbody2D rb;
    private KnockbackReceiverNew knockback;

    void Awake()
    {
        attack = GetComponent<GigantesAttack>();
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockbackReceiverNew>();
        if (knockback == null)
            knockback = gameObject.AddComponent<KnockbackReceiverNew>();
    }

    public void TickMovement(Transform target)
    {
        if (target == null) return;
        if (knockback != null && knockback.IsKnockedBack) return;

        // Pause movement while the attack windup/impact is active.
        if (attack != null && attack.IsCommitted)
            return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist <= stopDistance)
            return;

        Vector2 dir = (target.position - transform.position).normalized;
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
