using UnityEngine;
using System.Collections;

public class GigantesAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 3;
    public float range = 1.7f;
    public float knockbackForce = 6f;

    [Header("Timing")]
    public float windupTime = 0.6f;
    public float stuckTime = 1.0f;
    public float cooldown = 2.5f;

    private float lastAttack;
    private bool isAttacking;

    private Transform player;
    private Vector2 lockedTargetPosition;
    private EnemyState state;

    public bool IsCommitted => isAttacking;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        state = GetComponent<EnemyState>();
    }

    void Update()
    {
        if (player == null) return;
        if (state != null && !state.IsHostile) return;
        if (isAttacking) return;
        if (Time.time < lastAttack + cooldown) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > range) return;

        // Snapshot player position once to prevent last-second dodges.
        lockedTargetPosition = player.position;

        StartCoroutine(AttackRoutine());
    }

    // Required by interface, but NOT used externally
    public void TryAttack(Transform target)
    {
        // Intentionally empty: this enemy self-initiates attacks via Update.
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;

        yield return new WaitForSeconds(windupTime);

        Vector2 dir = (lockedTargetPosition - (Vector2)transform.position).normalized;

        HitResolver.ApplyHit(
            player.gameObject,
            damage,
            dir * knockbackForce,
            transform.position
        );

        yield return new WaitForSeconds(stuckTime);

        isAttacking = false;
        lastAttack = Time.time;

    }
}
