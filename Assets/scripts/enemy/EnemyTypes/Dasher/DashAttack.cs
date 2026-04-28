using UnityEngine;
public class DashAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 2;
    public float hitCooldown = 0.25f;
    public float knockbackForce = 4f;
    public float maxTargetHeightDelta = 0.75f;
    public bool requireGroundedTarget = true;

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
        if (!CanTarget(target))
            return;

        float x = Mathf.Sign(target.position.x - transform.position.x);
        if (Mathf.Approximately(x, 0f))
            x = transform.localScale.x >= 0f ? 1f : -1f;

        Vector2 dir = new Vector2(x, 0f);

        HitResolver.ApplyHit(
            target.gameObject,
            damage,
            dir * knockbackForce,
            transform.position
        );

        lastHit = Time.time;
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
