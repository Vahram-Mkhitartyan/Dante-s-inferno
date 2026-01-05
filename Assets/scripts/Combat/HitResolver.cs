using UnityEngine;

public static class HitResolver
{
    public static void ApplyHit(GameObject target, int damage, Vector2 force, Vector2 attackerPosition)
    {
        if (target == null) return;

        // Support colliders on child objects.
        Health hp = target.GetComponentInParent<Health>();
        if (hp) hp.TakeDamage(damage, attackerPosition);

        KnockbackReceiver kb = target.GetComponentInParent<KnockbackReceiver>();
        if (kb)
        {
            kb.ApplyKnockback(force, force.magnitude);
            return;
        }

        Rigidbody2D rb = target.GetComponentInParent<Rigidbody2D>();
        if (rb) rb.AddForce(force, ForceMode2D.Impulse);
    }
}
