using UnityEngine;

public class AttackExecutor : MonoBehaviour
{
    [Header("Hit Origin")]
    public Vector2 hitOffset = new Vector2(0.8f, 0f);
    public Transform hitOrigin;
    public LayerMask enemyLayer;

    [Header("Sword")]
    public float swordRange = 1.2f;
    public int swordDamage = 1;
    public float swordKnockback = 4f;

    [Header("Kick")]
    public int kickDamage = 1;
    public float kickHorizontalForce = 4f;
    public float kickVerticalForce = 8f;


    [Header("Spin")]
    public float spinRadius = 1.5f;
    public int spinDamage = 2;

    public void Execute(AttackType attack)
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
        Collider2D hit = Physics2D.OverlapCircle(
            hitOrigin.position, swordRange, enemyLayer);

        if (!hit) return;
        Apply(hit, swordDamage, Direction(hit) * swordKnockback);
    }

    void Kick()
    {
        Collider2D hit = Physics2D.OverlapCircle(
            GetHitOrigin(),
            1f,
            enemyLayer
        );

        if (!hit) return;

        float dir = GetFacingDirection();
        Vector2 force = new Vector2(dir * kickHorizontalForce, kickVerticalForce);

        Apply(hit, kickDamage, force);
    }

    float GetFacingDirection()
    {
        PlayerController pc = GetComponent<PlayerController>();
        return pc != null ? pc.FacingDirection : 1f;
    }



    void Spin()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position, spinRadius, enemyLayer);

        foreach (var h in hits)
            Apply(h, spinDamage, Direction(h) * 5f);
    }

    void Apply(Collider2D target, int dmg, Vector2 force)
    {
        Health hp = target.GetComponent<Health>();
        if (hp) hp.TakeDamage(dmg);

        KnockbackReceiver kb = target.GetComponent<KnockbackReceiver>();
        if (kb) kb.ApplyKnockback(force, force.magnitude);
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
}
