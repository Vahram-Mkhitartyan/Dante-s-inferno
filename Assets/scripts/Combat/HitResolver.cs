using UnityEngine;

public static class HitResolver
{
    public static void ApplyHit(GameObject target, int damage, Vector2 force, Vector2 attackerPosition)
    {
        if (target == null) return;

        // Support colliders on child objects.
        Health hp = target.GetComponentInParent<Health>();
        if (hp) hp.TakeDamage(damage, attackerPosition);

        if (force != Vector2.zero)
            KnockbackSystemNew.Apply(hp != null ? hp.gameObject : target, force.normalized, force.magnitude);
    }
}
