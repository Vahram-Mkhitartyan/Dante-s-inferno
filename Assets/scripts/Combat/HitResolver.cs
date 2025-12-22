using UnityEngine;

public static class HitResolver
{
    public static void ApplyHit(GameObject target, int damage, Vector2 force, Vector2 attackerPosition)
    {
        if (target == null) return;

        Health hp = target.GetComponent<Health>();
        if (hp) hp.TakeDamage(damage, attackerPosition);

        KnockbackReceiver kb = target.GetComponent<KnockbackReceiver>();
        if (kb) kb.ApplyKnockback(force, force.magnitude);
    }
}
