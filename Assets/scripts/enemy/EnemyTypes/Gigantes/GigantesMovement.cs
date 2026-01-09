using UnityEngine;

public class GigantesMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 1.1f;
    public float stopDistance = 1.6f;

    private GigantesAttack attack;
    private Rigidbody2D rb;

    void Awake()
    {
        attack = GetComponent<GigantesAttack>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void TickMovement(Transform target)
    {
        if (target == null) return;

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
        if (rb == null || rb.bodyType == RigidbodyType2D.Static || rb.constraints != RigidbodyConstraints2D.None)
        {
            transform.position += (Vector3)delta;
            return;
        }

        rb.MovePosition(rb.position + delta);
    }
}
