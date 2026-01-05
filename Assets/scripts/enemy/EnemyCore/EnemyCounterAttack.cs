using UnityEngine;

public class EnemyCounterAttack : MonoBehaviour
{
    public ComboQueue playerCombo;
    public float cooldown = 2f;
    public int spamRepeatThreshold = 3;
    public float spamHitMultiplier = 1.4f;
    public float spamPenaltyDuration = 0.6f;

    private IEnemyAttack enemyAttack;
    private float lastCounterTime;

    void Awake()
    {
        enemyAttack = GetComponent<IEnemyAttack>();
    }

    void Update()
    {
        if (enemyAttack == null || playerCombo == null)
            return;

        if (Time.time < lastCounterTime + cooldown)
            return;

        if (playerCombo.IsSameSpam(spamRepeatThreshold) ||
            playerCombo.IsAlternatingSpam(spamRepeatThreshold))
        {
            Counter();
            playerCombo.Clear();
            lastCounterTime = Time.time;
        }
    }

    void Counter()
    {
        enemyAttack.TryAttack(playerCombo.transform);
    }
}
