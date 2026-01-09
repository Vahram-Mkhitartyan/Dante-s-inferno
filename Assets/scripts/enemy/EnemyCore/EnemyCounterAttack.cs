using UnityEngine;

public class EnemyCounterAttack : MonoBehaviour
{
    public ComboQueue playerCombo;
    public float cooldown = 2f;
    public int spamRepeatThreshold = 3;
    public float spamHitMultiplier = 1.4f;
    public float spamPenaltyDuration = 0.6f;
    public float counterKnockbackForce = 6f;
    public bool debugLogs = true;

    private IEnemyAttack enemyAttack;
    private EnemyState state;
    private float lastCounterTime;
    private bool loggedMissingRefs;
    private float nextDebugTime;

    void Awake()
    {
        enemyAttack = GetComponent<IEnemyAttack>();
        state = GetComponent<EnemyState>();

        if (playerCombo == null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
                playerCombo = player.GetComponent<ComboQueue>();
        }

        if (debugLogs)
        {
            string comboInfo = playerCombo != null ? $"{playerCombo.name}#{playerCombo.GetInstanceID()}" : "null";
        }
    }

    void OnEnable()
    {
        if (debugLogs)
    }

    void Update()
    {
        if (playerCombo == null)
        {
            Transform player = GameObject.FindGameObjectWithTag("Player")?.transform;
            if (player != null)
                playerCombo = player.GetComponent<ComboQueue>();
        }

        if (enemyAttack == null || playerCombo == null)
        {
            if (!loggedMissingRefs && debugLogs)
            {
                loggedMissingRefs = true;
            }
            return;
        }

        bool hostile = state == null || state.IsHostile;
        if (!hostile)
        {
            if (debugLogs && Time.time >= nextDebugTime)
            {
                nextDebugTime = Time.time + 0.5f;
            }
            return;
        }

        if (Time.time < lastCounterTime + cooldown)
        {
            if (debugLogs && Time.time >= nextDebugTime)
            {
                float remaining = lastCounterTime + cooldown - Time.time;
                nextDebugTime = Time.time + 0.5f;
            }
            return;
        }

        bool same = playerCombo.IsSameSpam(spamRepeatThreshold);
        bool alt = playerCombo.IsAlternatingSpam(spamRepeatThreshold);

        string last3 = playerCombo.DebugStringLast(3);
        string last6 = playerCombo.DebugStringLast(6);
        if (debugLogs && Time.time >= nextDebugTime)
        {
            string comboInfo = playerCombo != null ? $"{playerCombo.name}#{playerCombo.GetInstanceID()}" : "null";
            nextDebugTime = Time.time + 0.5f;
        }

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
        if (debugLogs)
        enemyAttack.TryAttack(target);

        KnockbackReceiver kb = target.GetComponentInParent<KnockbackReceiver>();
        if (kb != null)
        {
            Vector2 dir = (target.position - transform.position).normalized;
            kb.ApplyKnockback(dir, counterKnockbackForce);
        }
    }
}
