using UnityEngine;
public class DashAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 2;
    public float hitCooldown = 0.25f;

    private float lastHit;

    public void TryAttack(Transform target)
    {
        if (Time.time < lastHit + hitCooldown)
            return;

        Vector2 dir = (target.position - transform.position).normalized;

        HitResolver.ApplyHit(
            target.gameObject,
            damage,
            dir,
            transform.position
        );

        lastHit = Time.time;
    }
}
