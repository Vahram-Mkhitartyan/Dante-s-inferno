using UnityEngine;

public class KnockbackGiver : MonoBehaviour
{
    [Header("Knockback Settings")]
    public float baseForce = 6f;
    public float comboForceBonus = 2f;
    public float verticalForce = 0f;

    [Header("Direction")]
    public bool pushAwayFromSelf = true;

    public void ApplyTo(GameObject target, int comboStep = 0)
    {
        KnockbackReceiver receiver = target.GetComponent<KnockbackReceiver>();
        if (receiver == null) return;

        Vector2 direction = GetDirection(target.transform);
        float force = baseForce + comboForceBonus * comboStep;

        Vector2 finalForce = direction * force;

        if (verticalForce > 0f)
            finalForce += Vector2.up * verticalForce;

        receiver.ApplyKnockback(finalForce.normalized, finalForce.magnitude);

    }

    Vector2 GetDirection(Transform target)
    {
        if (!pushAwayFromSelf)
            return Vector2.right;

        float dir = Mathf.Sign(target.position.x - transform.position.x);
        return new Vector2(dir, 0f);
    }
}
