using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 0.8f;
    public float attackCooldown = 1.2f;

    private Transform player;
    private float lastAttackTime;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (!player) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            Attack();
        }
    }

    void Attack()
    {
        lastAttackTime = Time.time;

        Health health = player.GetComponent<Health>();
        if (health)
            health.TakeDamage(damage);

        KnockbackReceiver knockback = player.GetComponent<KnockbackReceiver>();
        if (knockback)
        {
            Vector2 dir = (player.position - transform.position);
            knockback.ApplyKnockback(dir, 5f);
        }
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
