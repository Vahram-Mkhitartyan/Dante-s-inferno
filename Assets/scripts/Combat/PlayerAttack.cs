using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 1f;
    public Vector2 attackOffset = new Vector2(0.8f, 0f);
    public LayerMask enemyLayer;
    private KnockbackGiver knockbackGiver;

    private PlayerController player;

    void Awake()
    {
        player = GetComponent<PlayerController>();
        knockbackGiver = GetComponent<KnockbackGiver>();

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Attack();
        }
    }

    void Attack()
    {
        Vector2 attackPosition = (Vector2)transform.position +
            new Vector2(attackOffset.x * player.FacingDirection, attackOffset.y);

        Collider2D hit = Physics2D.OverlapCircle(
            attackPosition,
            attackRange,
            enemyLayer
        );

        if (hit)
        {
            Health health = hit.GetComponent<Health>();
            if (health)
                health.TakeDamage(damage);

            if (knockbackGiver)
            {
                knockbackGiver.ApplyTo(hit.gameObject, 0); // comboStep = 0 for now
            }
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        float dir = 1f;
        PlayerController pc = GetComponent<PlayerController>();
        if (pc) dir = pc.FacingDirection;

        Vector2 gizmoPos = (Vector2)transform.position +
                           new Vector2(attackOffset.x * dir, attackOffset.y);

        Gizmos.DrawWireSphere(gizmoPos, attackRange);
    }
}
