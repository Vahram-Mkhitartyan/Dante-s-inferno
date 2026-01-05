using UnityEngine;
using System.Collections.Generic;

public class ThiefAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Attack")]
    public int damage = 1;
    public float range = 1.2f;
    public float cooldown = 1.8f;

    [Header("Steal")]
    [Range(0f, 1f)]
    public float stealChance = 0.10f;

    [Header("Flee")]
    public float fleeDuration = 1.5f;

    private float lastAttack;

    private Transform player;
    private Health playerHealth;
    private GearEquipper playerGear;
    private ThiefMovement thiefMovement;
    private Health thiefHealth;

    private bool lastHitWasBlocked;

    // 🧠 Stored stolen gear (only one piece per thief)
    private System.Action restoreAction;
    private bool hasStolen;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            Debug.LogError("[ThiefAttack] Player not found!");
            return;
        }

        // Player health (block detection)
        playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnBlocked += () =>
            {
                lastHitWasBlocked = true;
            };
        }

        // Player gear (indirect, untouched)
        playerGear = player.GetComponentInChildren<GearEquipper>();

        // Movement
        thiefMovement = GetComponent<ThiefMovement>();

        // Thief health (for death restore)
        thiefHealth = GetComponent<Health>();
        if (thiefHealth != null)
        {
            thiefHealth.OnDeath += OnThiefDeath;
        }
    }

    void Update()
    {
        if (player == null) return;
        if (Time.time < lastAttack + cooldown) return;

        float dist = Vector2.Distance(transform.position, player.position);
        if (dist > range) return;

        TryAttack(player);
    }

    public void TryAttack(Transform target)
    {
        lastHitWasBlocked = false;
        lastAttack = Time.time;

        Vector2 dir = (target.position - transform.position).normalized;
        HitResolver.ApplyHit(
            target.gameObject,
            damage,
            dir,
            transform.position
        );

        // Attempt steal only if:
        // - not blocked
        // - hasn't already stolen
        if (!lastHitWasBlocked && !hasStolen && playerGear != null && Random.value <= stealChance)
        {
            TryStealArmor(playerGear);
        }
    }

    bool TryStealArmor(GearEquipper gear)
    {
        List<System.Action> stealActions = new List<System.Action>();
        List<System.Action> restoreActions = new List<System.Action>();

        if (gear.Helmet != 0)
        {
            int v = gear.Helmet;
            stealActions.Add(() => gear.Helmet = 0);
            restoreActions.Add(() => gear.Helmet = v);
        }
        if (gear.Shoulder != 0)
        {
            int v = gear.Shoulder;
            stealActions.Add(() => gear.Shoulder = 0);
            restoreActions.Add(() => gear.Shoulder = v);
        }
        if (gear.Armor != 0)
        {
            int v = gear.Armor;
            stealActions.Add(() => gear.Armor = 0);
            restoreActions.Add(() => gear.Armor = v);
        }
        if (gear.Arm != 0)
        {
            int v = gear.Arm;
            stealActions.Add(() => gear.Arm = 0);
            restoreActions.Add(() => gear.Arm = v);
        }
        if (gear.Feet != 0)
        {
            int v = gear.Feet;
            stealActions.Add(() => gear.Feet = 0);
            restoreActions.Add(() => gear.Feet = v);
        }

        if (stealActions.Count == 0)
            return false;

        int index = Random.Range(0, stealActions.Count);

        // Steal
        stealActions[index].Invoke();
        gear.ApplySkinChanges();

        // Store restore action
        restoreAction = restoreActions[index];
        hasStolen = true;

        // Flee
        thiefMovement?.TriggerFlee(fleeDuration);

        Debug.Log("[ThiefAttack] 🩸 Stole gear — recoverable on death");
        return true;
    }

    void OnThiefDeath()
    {
        if (!hasStolen || restoreAction == null || playerGear == null)
            return;

        restoreAction.Invoke();
        playerGear.ApplySkinChanges();

        Debug.Log("[ThiefAttack] ☠️ Thief killed — gear returned");
    }
}
