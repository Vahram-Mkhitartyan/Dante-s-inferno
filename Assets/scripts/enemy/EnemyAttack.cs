using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int damage = 1;
    public float attackRange = 0.8f;
    public float attackCooldown = 1.2f;

    private Transform player;
    private Health playerHealth;
    private Health selfHealth;

    private float lastAttackTime;
    private bool isDisabled;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player)
            playerHealth = player.GetComponent<Health>();

        selfHealth = GetComponent<Health>();

        // 🔒 Disable this attacker permanently on death
        if (selfHealth != null)
            selfHealth.OnDeath += DisableAttack;
    }

    void Update()
    {
        if (isDisabled) return;
        if (!player || playerHealth == null) return;

        // ❗ DO NOT attack dead player
        if (playerHealth.IsDead) return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        lastAttackTime = Time.time;
        DealDamage();
    }

    // 🔹 Counterattack entry
    public void ForceAttack()
    {
        if (isDisabled) return;
        if (!player || playerHealth == null) return;
        if (playerHealth.IsDead) return;

        lastAttackTime = Time.time;
        DealDamage();
    }

    void DealDamage()
    {
        if (playerHealth == null || playerHealth.IsDead) return;

        playerHealth.TakeDamage(damage, transform.position);

        KnockbackReceiver knockback = player.GetComponent<KnockbackReceiver>();
        if (knockback)
        {
            Vector2 dir = (player.position - transform.position).normalized;
            knockback.ApplyKnockback(dir, 5f);
        }
    }

    void DisableAttack()
    {
        isDisabled = true;
        enabled = false; // hard stop
    }
}
