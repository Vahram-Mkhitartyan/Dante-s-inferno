using UnityEngine;
using System.Collections.Generic;

public class ThiefAttack : MonoBehaviour, IEnemyAttack
{
    [Header("Attack")]
    public int damage = 1;
    public float range = 1.2f;
    public float cooldown = 1.8f;
    public float knockbackForce = 3.5f;

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
    private EnemyState state;

    private bool lastHitWasBlocked;

    // Stores a single stolen gear slot so it can be restored on death.
    private GearSlot stolenSlot = GearSlot.None;
    private int stolenValue;
    private bool hasStolen;

    private enum GearSlot
    {
        None,
        Helmet,
        Shoulder,
        Armor,
        Arm,
        Feet
    }

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
        if (player == null)
        {
            return;
        }

        // Listen for block events to avoid stealing on blocked hits.
        playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.OnBlocked += OnPlayerBlocked;
        }

        // Access player gear from children (if present) to steal a piece.
        playerGear = player.GetComponentInChildren<GearEquipper>();

        // Movement used for flee behavior after a successful steal.
        thiefMovement = GetComponent<ThiefMovement>();

        // Restore stolen gear when the thief dies.
        thiefHealth = GetComponent<Health>();
        if (thiefHealth != null)
        {
            thiefHealth.OnDeath += OnThiefDeath;
        }

        state = GetComponent<EnemyState>();
    }

    void OnDestroy()
    {
        if (playerHealth != null)
            playerHealth.OnBlocked -= OnPlayerBlocked;
        if (thiefHealth != null)
            thiefHealth.OnDeath -= OnThiefDeath;
    }

    void Update()
    {
        if (player == null) return;
        if (state != null && !state.IsHostile) return;
        if (Time.time < lastAttack + cooldown) return;

        float rangeSqr = range * range;
        if (((Vector2)transform.position - (Vector2)player.position).sqrMagnitude > rangeSqr)
            return;

        TryAttack(player);
    }

    public void TryAttack(Transform target)
    {
        if (state != null && !state.IsHostile) return;
        lastHitWasBlocked = false;
        lastAttack = Time.time;

        Vector2 dir = (target.position - transform.position).normalized;
        HitResolver.ApplyHit(
            target.gameObject,
            damage,
            dir * knockbackForce,
            transform.position
        );

        // Attempt steal only if the hit landed and the thief hasn't stolen yet.
        if (!lastHitWasBlocked && !hasStolen && playerGear != null && Random.value <= stealChance)
        {
            TryStealArmor(playerGear);
        }
    }

    bool TryStealArmor(GearEquipper gear)
    {
        GearSlot[] slots = new GearSlot[5];
        int count = 0;

        if (gear.Helmet != 0)
        {
            slots[count++] = GearSlot.Helmet;
        }
        if (gear.Shoulder != 0)
        {
            slots[count++] = GearSlot.Shoulder;
        }
        if (gear.Armor != 0)
        {
            slots[count++] = GearSlot.Armor;
        }
        if (gear.Arm != 0)
        {
            slots[count++] = GearSlot.Arm;
        }
        if (gear.Feet != 0)
        {
            slots[count++] = GearSlot.Feet;
        }

        if (count == 0)
            return false;

        int index = Random.Range(0, count);
        GearSlot selected = slots[index];

        // Pick one gear slot to steal, then store its restore info.
        switch (selected)
        {
            case GearSlot.Helmet:
                stolenValue = gear.Helmet;
                gear.Helmet = 0;
                break;
            case GearSlot.Shoulder:
                stolenValue = gear.Shoulder;
                gear.Shoulder = 0;
                break;
            case GearSlot.Armor:
                stolenValue = gear.Armor;
                gear.Armor = 0;
                break;
            case GearSlot.Arm:
                stolenValue = gear.Arm;
                gear.Arm = 0;
                break;
            case GearSlot.Feet:
                stolenValue = gear.Feet;
                gear.Feet = 0;
                break;
        }
        gear.ApplySkinChanges();

        stolenSlot = selected;
        hasStolen = true;

        // Flee briefly after a successful steal.
        thiefMovement?.TriggerFlee(fleeDuration);

        return true;
    }

    void OnThiefDeath()
    {
        if (!hasStolen || stolenSlot == GearSlot.None || playerGear == null)
            return;

        // Return the stolen gear piece on death.
        switch (stolenSlot)
        {
            case GearSlot.Helmet:
                playerGear.Helmet = stolenValue;
                break;
            case GearSlot.Shoulder:
                playerGear.Shoulder = stolenValue;
                break;
            case GearSlot.Armor:
                playerGear.Armor = stolenValue;
                break;
            case GearSlot.Arm:
                playerGear.Arm = stolenValue;
                break;
            case GearSlot.Feet:
                playerGear.Feet = stolenValue;
                break;
        }
        playerGear.ApplySkinChanges();

    }

    void OnPlayerBlocked()
    {
        lastHitWasBlocked = true;
    }
}
