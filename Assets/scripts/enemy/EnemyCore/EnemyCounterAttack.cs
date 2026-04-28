using UnityEngine;

public class EnemyCounterAttack : MonoBehaviour
{
    public ComboQueue playerCombo;
    public float cooldown = 2f;
    public int spamRepeatThreshold = 3;
    public float spamHitMultiplier = 1.4f;
    public float spamPenaltyDuration = 0.6f;

    private IEnemyAttack enemyAttack;
    private EnemyState state;
    private AttackExecutor playerAttackExecutor;
    private float lastCounterTime;

    void Awake()
    {
        enemyAttack = GetComponent<IEnemyAttack>();
        state = GetComponent<EnemyState>();

        if (playerCombo == null)
        {
            ResolvePlayerReferences();
        }
        else
        {
            playerAttackExecutor = playerCombo.GetComponent<AttackExecutor>();
        }
    }

    void Update()
    {
        if (playerCombo == null)
            ResolvePlayerReferences();

        if (enemyAttack == null || playerCombo == null)
            return;

        bool hostile = state == null || state.IsHostile;
        if (!hostile)
            return;

        if (Time.time < lastCounterTime + cooldown)
            return;

        bool same = playerCombo.IsSameSpam(spamRepeatThreshold);
        bool alt = playerCombo.IsAlternatingSpam(spamRepeatThreshold);

        if (same || alt)
        {
            Counter();
            playerCombo.Clear();
            lastCounterTime = Time.time;
        }
    }

    void Counter()
    {
        Transform target = playerCombo.GetComponentInParent<Health>()?.transform ?? playerCombo.transform;
        playerAttackExecutor?.ApplyHitDelayPenalty(spamHitMultiplier, spamPenaltyDuration);
        enemyAttack.TryAttack(target);
    }

    void ResolvePlayerReferences()
    {
        Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
            return;

        playerCombo = player.GetComponent<ComboQueue>();
        playerAttackExecutor = player.GetComponent<AttackExecutor>();
    }
}
