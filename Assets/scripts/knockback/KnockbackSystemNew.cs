using UnityEngine;

public static class KnockbackSystemNew
{
    public static bool Apply(GameObject target, Vector2 direction, float baseForce)
    {
        if (target == null) return false;

        KnockbackReceiverNew receiver = target.GetComponentInParent<KnockbackReceiverNew>();
        if (receiver == null) return false;

        receiver.Apply(direction, baseForce);
        return true;
    }

    public static bool ApplyFromAttacker(GameObject target, Vector2 attackerPosition, float baseForce)
    {
        if (target == null) return false;

        Vector2 dir = ((Vector2)target.transform.position - attackerPosition).normalized;
        return Apply(target, dir, baseForce);
    }
}
