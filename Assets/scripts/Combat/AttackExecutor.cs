using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackExecutor : MonoBehaviour
{
    [Header("Queue")]
    public int maxQueuedAttacks = 3;

    [Header("Hit Timing Modifiers")]
    public float hitDelayMultiplier = 1f;

    [Header("Hit Origin")]
    public Vector2 hitOffset = new Vector2(0.8f, 0f);
    public Transform hitOrigin;
    public LayerMask enemyLayer;

    [Header("Sword")]
    public float swordRange = 1.2f;
    public int swordDamage = 1;
    public float swordKnockback = 4f;
    public float swordHitDelay = 0.15f;

    [Header("Kick")]
    public int kickDamage = 1;
    public float kickHorizontalForce = 4f;
    public float kickVerticalForce = 8f;
    public float kickHitDelay = 0.18f;


    [Header("Spin")]
    public float spinRadius = 1.5f;
    public int spinDamage = 2;
    public float spinHitDelay = 0.2f;


    [SerializeField] private PlayerSpineAnimationController animControl;
    private readonly Queue<AttackType> queued = new Queue<AttackType>();
    private Coroutine hitDelayRoutine;

    void Update()
    {
        if (animControl && animControl.IsDead())
        {
            queued.Clear();
            return;
        }

        if (queued.Count == 0) return;
        if (animControl && animControl.IsLocked()) return;

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
        switch (attack)
        {
            case AttackType.Sword:
                Sword();
                break;
            case AttackType.KickLauncher:
                Kick();
                break;
            case AttackType.Spin360:
                Spin();
                break;
        }
    }

    void Sword()
    {
        animControl.RequestAttack("Attack1", 0.6f);
        StartCoroutine(SwordHitRoutine());
    }

    void Kick()
    {
        animControl.RequestAttack("Buff", 0.6f);
        StartCoroutine(KickHitRoutine());
    }

    float GetFacingDirection()
    {
        PlayerController pc = GetComponent<PlayerController>();
        return pc != null ? pc.FacingDirection : 1f;
    }



    void Spin()
    {
        animControl.RequestAttack("Attack2", 0.6f);
        StartCoroutine(SpinHitRoutine());
    }

    IEnumerator SwordHitRoutine()
    {
        float delay = swordHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(
            hitOrigin.position, swordRange, enemyLayer);

        if (!hit) yield break;
        Apply(hit, swordDamage, Direction(hit) * swordKnockback);
    }

    IEnumerator KickHitRoutine()
    {
        float delay = kickHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D hit = Physics2D.OverlapCircle(
            GetHitOrigin(),
            1f,
            enemyLayer
        );

        if (!hit) yield break;

        float dir = GetFacingDirection();
        Vector2 force = new Vector2(dir * kickHorizontalForce, kickVerticalForce);

        Apply(hit, kickDamage, force);
    }

    IEnumerator SpinHitRoutine()
    {
        float delay = spinHitDelay * hitDelayMultiplier;
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, spinRadius, enemyLayer);

        foreach (var h in hits)
            Apply(h, spinDamage, Direction(h) * 5f);
    }

    void Apply(Collider2D target, int dmg, Vector2 force)
    {
        HitResolver.ApplyHit(target.gameObject, dmg, force, transform.position);
    }

    Vector2 GetHitOrigin()
    {
        float dir = GetFacingDirection();
        return (Vector2)transform.position + new Vector2(dir * hitOffset.x, hitOffset.y);
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
}
