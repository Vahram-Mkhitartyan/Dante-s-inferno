using UnityEngine;

public class EnemyCounterAttack : MonoBehaviour
{
    public ComboQueue playerCombo;
    public float cooldown = 2f;
    public int spamRepeatThreshold = 3;
    public AttackExecutor playerAttackExecutor;
    public float spamHitDelayMultiplier = 1.4f;
    public float spamPenaltyDuration = 0.6f;

    private EnemyAttack enemyAttack;
    private float lastCounterTime;

    void Awake()
    {
        Debug.Log("EnemyCounterAttack AWAKE on ");
        enemyAttack = GetComponent<EnemyAttack>();
    }

    void Update()
    {
        if (!playerCombo) return;
        if (Time.time < lastCounterTime + cooldown) return;

        if (playerCombo.IsSameSpam(spamRepeatThreshold) || playerCombo.IsAlternatingSpam(spamRepeatThreshold))
        {
            Debug.Log("COUNTER TRIGGERED. Combo = " + playerCombo.DebugString());
            Counter();
            if (playerAttackExecutor)
                playerAttackExecutor.ApplyHitDelayPenalty(spamHitDelayMultiplier, spamPenaltyDuration);
            playerCombo.Clear();   // punish ONCE
            lastCounterTime = Time.time;
        }
    }

    void Counter()
    {
        if (!enemyAttack) return;

        enemyAttack.ForceAttack();
    }
}
