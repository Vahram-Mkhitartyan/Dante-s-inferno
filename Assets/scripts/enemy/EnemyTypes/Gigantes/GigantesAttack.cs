using UnityEngine;
using System.Collections;

public class GigantesAttack : MonoBehaviour, IEnemyAttack
{
    public int damage = 3;
    public float range = 1.7f;

    [Header("Timing")]
    public float windupTime = 0.6f;
    public float stuckTime = 1.0f;
    public float cooldown = 2.5f;

    private float lastAttack;
    private bool isAttacking;

    private Transform player;
    private Vector2 lockedTargetPosition;

    public bool IsCommitted => isAttacking;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (player == null) return;
        if (isAttacking) return;
        if (Time.time < lastAttack + cooldown) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > range) return;

        // Snapshot once
        lockedTargetPosition = player.position;

        Debug.Log($"[GigantesAttack] 🎯 SNAPSHOT at {lockedTargetPosition}");

        StartCoroutine(AttackRoutine());
    }

    // Required by interface, but NOT used externally
    public void TryAttack(Transform target)
    {
        // Intentionally empty or could redirect to Update logic
    }

    IEnumerator AttackRoutine()
    {
        isAttacking = true;
        Debug.Log("[GigantesAttack] ⚠️ WIND-UP");

        yield return new WaitForSeconds(windupTime);

        Vector2 dir = (lockedTargetPosition - (Vector2)transform.position).normalized;

        Debug.Log("[GigantesAttack] 💥 HIT");

        HitResolver.ApplyHit(
            player.gameObject,
            damage,
            dir,
            transform.position
        );

        Debug.Log("[GigantesAttack] 🧱 STUCK");

        yield return new WaitForSeconds(stuckTime);

        isAttacking = false;
        lastAttack = Time.time;

        Debug.Log("[GigantesAttack] 🔓 RECOVERED");
    }
}
