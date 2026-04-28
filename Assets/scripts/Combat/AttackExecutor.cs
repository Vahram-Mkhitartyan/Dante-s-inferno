using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;

public class AttackExecutor : MonoBehaviour
{
    //enemy layer
    private static readonly string[] EnemyLayerNames = { "enemy", "Enemy" };

    [Header("Queue")]
    public int maxQueuedAttacks = 3;

    [Header("Hit Timing Modifiers")]
    public float hitDelayMultiplier = 1f;

    [Header("Hit Origin")]
    public Vector2 hitOffset = new Vector2(0.8f, 0f);
    public Transform hitOrigin;
    public LayerMask enemyLayer;

    [Header("J - Top-Down Swing")]
    [FormerlySerializedAs("swordRange")] public float topDownRange = 1.15f;
    [FormerlySerializedAs("swordDamage")] public int topDownDamage = 1;
    [FormerlySerializedAs("swordKnockbackForce")] public float topDownKnockbackForce = 5.5f;
    public float topDownVerticalForce = -0.2f;
    [FormerlySerializedAs("swordHitDelay")] public float topDownHitDelay = 0.14f;

    [Header("K - Upward Launcher")]
    [FormerlySerializedAs("kickDamage")] public int upwardDamage = 1;
    [FormerlySerializedAs("kickKnockbackForce")] public float upwardKnockbackForce = 6.5f;
    [FormerlySerializedAs("kickVerticalLift")] public float upwardLift = 1.35f;
    [FormerlySerializedAs("kickHitDelay")] public float upwardHitDelay = 0.18f;

    [Header("L - Forward Pierce")]
    [FormerlySerializedAs("spinRadius")] public float pierceRange = 1.6f;
    public float pierceRadius = 0.45f;
    [FormerlySerializedAs("spinDamage")] public int pierceDamage = 2;
    [FormerlySerializedAs("spinKnockbackForce")] public float pierceKnockbackForce = 9f;
    public float pierceLift = 0.05f;
    [FormerlySerializedAs("spinHitDelay")] public float pierceHitDelay = 0.12f;

    //initialize queue to store attack types
    [SerializeField] private PlayerSpineAnimationController animControl;
    private readonly Queue<AttackType> queued = new Queue<AttackType>();
    private Coroutine hitDelayRoutine;
    private ComboQueue combo;
    private PlayerBehaviorTracker behavior;

    void Awake()
    {
        EnsureEnemyLayer();
        combo = GetComponent<ComboQueue>();
        behavior = GetComponent<PlayerBehaviorTracker>();
        if (behavior == null)
            behavior = gameObject.AddComponent<PlayerBehaviorTracker>();
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        EnsureEnemyLayer();
    }
#endif

    void Update()
    {
        if (animControl && animControl.IsDead())
        {
            queued.Clear();
            return;
        }

        if (queued.Count == 0) return;
        if (animControl && animControl.IsLocked()) return;

        // Buffered inputs execute as soon as the current attack unlocks.
        ExecuteNow(queued.Dequeue());
    }

    public void Execute(AttackType attack)
    {
        if (animControl && animControl.IsLocked())
        {
            if (queued.Count < maxQueuedAttacks)
                queued.Enqueue(attack);
            return;
        }

        ExecuteNow(attack);
    }

    void ExecuteNow(AttackType attack)
    {
        behavior?.RecordAttack(attack);
        PlayerActionEvents.RaiseAttackStarted(attack);

        switch (attack)
        {
            case AttackType.TopDownSwing:
                TopDownSwing();
                break;
            case AttackType.UpwardLauncher:
                UpwardLauncher();
                break;
            case AttackType.ForwardPierce:
                ForwardPierce();
                break;
        }
    }

    void TopDownSwing()
    {
        animControl.RequestAttack("Attack1", 0.6f);
        StartCoroutine(TopDownHitRoutine());
    }

    void UpwardLauncher()
    {
        animControl.RequestAttack("Buff", 0.6f);
        StartCoroutine(UpwardHitRoutine());
    }

    float GetFacingDirection()
    {
        PlayerController pc = GetComponent<PlayerController>();
        return pc != null ? pc.FacingDirection : 1f;
    }



    void ForwardPierce()
    {
        animControl.RequestAttack("Attack2", 0.6f);
        StartCoroutine(PierceHitRoutine());
    }

    IEnumerator TopDownHitRoutine()
    {
        float delay = topDownHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(
            GetConfiguredHitOrigin(), topDownRange, enemyLayer);

        if (!hit) yield break;
        RegisterCombo('A');
        Vector2 forceDir = (Direction(hit) + Vector2.up * topDownVerticalForce).normalized;
        Apply(hit, topDownDamage, forceDir * topDownKnockbackForce, AttackType.TopDownSwing);
    }

    IEnumerator UpwardHitRoutine()
    {
        float delay = upwardHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(
            GetHitOrigin(),
            1f,
            enemyLayer
        );

        if (!hit) yield break;

        RegisterCombo('B');
        Vector2 dir = Direction(hit);
        Vector2 forceDir = (dir + Vector2.up * upwardLift).normalized;
        Vector2 force = forceDir * upwardKnockbackForce;

        Apply(hit, upwardDamage, force, AttackType.UpwardLauncher);
    }

    IEnumerator PierceHitRoutine()
    {
        float delay = pierceHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            GetPierceOrigin(), pierceRadius, enemyLayer);

        if (hits.Length > 0)
            RegisterCombo('C');

        Vector2 forceDir = new Vector2(GetFacingDirection(), pierceLift).normalized;
        foreach (var h in hits)
            Apply(h, pierceDamage, forceDir * pierceKnockbackForce, AttackType.ForwardPierce);
    }

    void Apply(Collider2D target, int dmg, Vector2 force, AttackType attack)
    {
        int scaledDamage = PlayerActionEvents.ModifyOutgoingDamage(dmg);
        HitResolver.ApplyHit(target.gameObject, scaledDamage, force, transform.position);
        PlayerActionEvents.RaiseAttackHit(attack);
    }

    void RegisterCombo(char input)
    {
        combo?.Register(input);
    }

    Vector2 GetHitOrigin()
    {
        float dir = GetFacingDirection();
        return (Vector2)transform.position + new Vector2(dir * hitOffset.x, hitOffset.y);
    }

    Vector2 GetConfiguredHitOrigin()
    {
        return hitOrigin != null ? hitOrigin.position : GetHitOrigin();
    }

    Vector2 GetPierceOrigin()
    {
        float dir = GetFacingDirection();
        return (Vector2)transform.position + new Vector2(dir * pierceRange, hitOffset.y);
    }

    Vector2 Direction(Collider2D t)
    {
        return (t.transform.position - transform.position).normalized;
    }

    public void ApplyHitDelayPenalty(float multiplier, float duration)
    {
        if (hitDelayRoutine != null)
            StopCoroutine(hitDelayRoutine);
        hitDelayRoutine = StartCoroutine(HitDelayPenaltyRoutine(multiplier, duration));
    }

    IEnumerator HitDelayPenaltyRoutine(float multiplier, float duration)
    {
        float previous = hitDelayMultiplier;
        hitDelayMultiplier = Mathf.Max(0f, multiplier);
        yield return new WaitForSeconds(duration);
        hitDelayMultiplier = previous;
        hitDelayRoutine = null;
    }

    void EnsureEnemyLayer()
    {
        if (enemyLayer.value != 0) return;

        foreach (var name in EnemyLayerNames)
        {
            int mask = LayerMask.GetMask(name);
            if (mask != 0)
            {
                enemyLayer = mask;
                break;
            }
        }
    }
}
