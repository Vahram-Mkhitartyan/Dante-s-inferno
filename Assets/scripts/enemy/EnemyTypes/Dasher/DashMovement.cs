using UnityEngine;

public class DashMovement : MonoBehaviour
{
    public float dashSpeed = 12f;
    public float dashDistance = 4f;
    public float cooldown = 1.5f;

    private bool isDashing;
    private float lastDashTime;

    private IEnemyAttack attack;

    void Awake()
    {
        attack = GetComponent<IEnemyAttack>();
    }

    public void TickMovement(Transform target)
    {
        if (isDashing) return;
        if (Time.time < lastDashTime + cooldown) return;

        StartCoroutine(Dash(target));
    }

    private System.Collections.IEnumerator Dash(Transform target)
    {
        isDashing = true;

        float traveled = 0f;
        Vector2 dir = (target.position - transform.position).normalized;

        while (traveled < dashDistance)
        {
            float step = dashSpeed * Time.deltaTime;
            transform.position += (Vector3)(dir * step);
            traveled += step;
            yield return null;
        }

        isDashing = false;
        lastDashTime = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isDashing) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            attack?.TryAttack(collision.transform);
        }
    }
}
