using UnityEngine;
using System.Collections;

public class DashMovement : MonoBehaviour, IEnemyMovement
{
    public float dashSpeed = 12f;
    public float dashDistance = 4f;
    public float cooldown = 1.5f;
    public float hitRadius = 0.6f;

    private bool isDashing;
    private float lastDashTime;
    private bool hitThisDash;

    private IEnemyAttack attack;

    void Awake()
    {
        attack = GetComponent<IEnemyAttack>();
    }

    public void TickMovement(Transform target)
    {
        if (isDashing) return;
        if (Time.time < lastDashTime + cooldown) return;
        if (target == null) return;

        StartCoroutine(Dash(target));
    }

    private IEnumerator Dash(Transform target)
    {
        isDashing = true;
        hitThisDash = false;

        float traveled = 0f;
        Vector2 dir = (target.position - transform.position).normalized;

        while (traveled < dashDistance)
        {
            float step = dashSpeed * Time.deltaTime;
            transform.position += (Vector3)(dir * step);
            traveled += step;
            TryHitPlayer();
            yield return null;
        }

        isDashing = false;
        lastDashTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;
        if (!collision.gameObject.CompareTag("Player")) return;

        // SAME pattern as EnemyCounterAttack
        attack?.TryAttack(collision.transform);
    }

    private void TryHitPlayer()
    {
        if (hitThisDash) return;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, hitRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Player")) continue;
            attack?.TryAttack(hit.transform);
            hitThisDash = true;
            break;
        }
    }
}
