using UnityEngine;
using System.Collections;

public class DashMovement : MonoBehaviour, IEnemyMovement
{
    public float dashSpeed = 12f;
    public float dashDistance = 4f;
    public float cooldown = 1.5f;
    public float hitRadius = 0.6f;
    public float maxTargetHeightDelta = 0.75f;
    public bool requireGroundedTarget = true;

    private bool isDashing;
    private float lastDashTime;
    private bool hitThisDash;

    private IEnemyAttack attack;
    private KnockbackReceiverNew knockback;

    void Awake()
    {
        attack = GetComponent<IEnemyAttack>();
        knockback = GetComponent<KnockbackReceiverNew>();
        if (knockback == null)
            knockback = gameObject.AddComponent<KnockbackReceiverNew>();
    }

    public void TickMovement(Transform target)
    {
        if (knockback != null && knockback.IsKnockedBack) return;
        if (isDashing) return;
        if (Time.time < lastDashTime + cooldown) return;
        if (target == null) return;
        if (!CanTarget(target)) return;

        StartCoroutine(Dash(target));
    }

    private IEnumerator Dash(Transform target)
    {
        isDashing = true;
        hitThisDash = false;

        float traveled = 0f;
        float x = Mathf.Sign(target.position.x - transform.position.x);
        if (Mathf.Approximately(x, 0f))
            x = transform.localScale.x >= 0f ? 1f : -1f;

        Vector2 dir = new Vector2(x, 0f);

        while (traveled < dashDistance)
        {
            if (knockback != null && knockback.IsKnockedBack)
                break;

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
        if (!CanTarget(collision.transform)) return;

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
            if (!CanTarget(hit.transform)) continue;
            attack?.TryAttack(hit.transform);
            hitThisDash = true;
            break;
        }
    }

    bool CanTarget(Transform target)
    {
        float heightDelta = target.position.y - transform.position.y;
        if (heightDelta > maxTargetHeightDelta)
            return false;

        if (!requireGroundedTarget)
            return true;

        PlayerController player = target.GetComponentInParent<PlayerController>();
        return player == null || player.IsGrounded;
    }
}
