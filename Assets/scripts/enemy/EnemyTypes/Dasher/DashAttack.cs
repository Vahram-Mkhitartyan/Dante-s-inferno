using UnityEngine;
public class DashAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 2;
    public float hitCooldown = 0.25f;
    public float knockbackForce = 4f;

    private float lastHit;
    private EnemyState state;

    void Awake()
    {
        state = GetComponent<EnemyState>();
    }

    public void TryAttack(Transform target)
    {
        if (state != null && !state.IsHostile)
            return;
        if (Time.time < lastHit + hitCooldown)
            return;

        Vector2 dir = (target.position - transform.position).normalized;

        HitResolver.ApplyHit(
            target.gameObject,
            damage,
            dir * knockbackForce,
            transform.position
        );

        lastHit = Time.time;
    }
}
