using UnityEngine;

public class BackStabber : MonoBehaviour
{
    [Header("Backstab Settings")]
    public float triggerDistance = 3f;
    public float windupTime = 0.15f;
    public int damage = 1;
    public float knockbackForce = 5f;

    private bool isPotentialTraitor = false;
    private bool armed = false;
    private bool triggered = false;

    private Transform player;
    private EnemyState enemyState;
    private PlayerController playerController;

    public bool IsPotentialTraitor => isPotentialTraitor;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player != null)
        {
            playerController = player.GetComponent<PlayerController>();
        }
        enemyState = GetComponent<EnemyState>();
    }

    public void SetPotentialTraitor(bool value)
    {
        isPotentialTraitor = value;
    }

    public void Arm(bool value)
    {
        armed = value;
        triggered = false;
    }

    void Update()
    {
        if (!armed || !isPotentialTraitor || triggered || player == null || enemyState == null)
            return;

        Vector2 toPlayer = player.position - transform.position;

        if (toPlayer.magnitude > triggerDistance)
            return;

        Vector2 toEnemy = transform.position - player.position;
        float facing = playerController != null ? playerController.FacingDirection : Mathf.Sign(player.localScale.x);
        if (Mathf.Approximately(facing, 0f))
            facing = 1f;

        // Backstab only when enemy is behind the player's facing direction.
        if (toEnemy.x * facing < 0f)
        {
            triggered = true;
            StartCoroutine(ExecuteBackstab());
        }
    }

    private System.Collections.IEnumerator ExecuteBackstab()
    {
        triggered = true;
        yield return new WaitForSeconds(windupTime);
        enemyState.SetHostile(true);
    }
}
