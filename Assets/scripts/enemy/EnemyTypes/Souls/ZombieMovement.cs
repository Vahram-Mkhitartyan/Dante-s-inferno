using UnityEngine;

public class ZombieMovement : MonoBehaviour, IEnemyMovement
{
    public float moveSpeed = 1.2f;

    private Rigidbody2D rb;
    private KnockbackReceiverNew knockback;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        knockback = GetComponent<KnockbackReceiverNew>();
    }

    public void TickMovement(Transform target)
    {
        if (rb == null || target == null)
            return;

        if (knockback != null && knockback.IsKnockedBack)
            return;

        Vector2 dir = (target.position - transform.position).normalized;
        rb.linearVelocity = new Vector2(dir.x * moveSpeed, rb.linearVelocity.y);

        // Face direction
        if (dir.x != 0)
        {
            Vector3 scale = transform.localScale;
            scale.x = Mathf.Sign(dir.x) * Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }
}
