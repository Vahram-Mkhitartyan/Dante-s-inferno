using UnityEngine;

public class ZombieAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 1;
    public float attackCooldown = 1.0f;
    public float attackRange = 0.9f;
    public float knockbackForce = 4f;

    private float lastAttackTime;

    public void TryAttack(Transform target)
    {
        if (Time.time < lastAttackTime + attackCooldown)
            return;

        if (target == null)
            return;

        float dist = Vector2.Distance(transform.position, target.position);
        if (dist > attackRange)
            return;

        Vector2 dir = (target.position - transform.position).normalized;
        HitResolver.ApplyHit(target.gameObject, damage, dir * knockbackForce, transform.position);

        lastAttackTime = Time.time;
    }
}
