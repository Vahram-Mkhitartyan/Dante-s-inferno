using UnityEngine;
using System.Collections;

public class Backstabber : MonoBehaviour
{
    [Header("Backstab Settings")]
    public float triggerDistance = 3.0f;
    public float windupTime = 0.15f;
    public int damage = 1;
    public float knockbackForce = 5f;


    // Identity (SET ONCE)
    public bool IsPotentialTraitor { get; private set; }

    // State (changes)
    private bool armed = false;
    private bool triggered = false;

    private EnemyState state;
    private Transform player;

    private float lastDot;
    private bool hasDot = false;

    void Awake()
    {
        state = GetComponent<EnemyState>();
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    // Called ONCE at scene start
    public void SetPotentialTraitor(bool value)
    {
        IsPotentialTraitor = value;
    }

    // Called whenever sin / world changes
    public void Arm(bool value)
    {
        armed = value;
        triggered = false;
        hasDot = false;
    }

    void Update()
    {
        if (!armed || triggered) return;
        if (!IsPotentialTraitor) return;
        if (!player || state == null) return;
        if (state.IsHostile) return;

        Vector2 toPlayer = player.position - transform.position;

        // Distance gate (THIS is what triggerDistance is for)
        if (toPlayer.magnitude > triggerDistance)
        {
            hasDot = false;
            return;
        }

        // Side check (left or right)
        float currentSide = Mathf.Sign(toPlayer.x);

        if (!hasDot)
        {
            lastDot = currentSide;
            hasDot = true;
            return;
        }

        // Crossing detection
        if (lastDot != currentSide)
        {
            triggered = true;
            StartCoroutine(Betray());
            return;
        }

        lastDot = currentSide;
    }




    IEnumerator Betray()
    {
        yield return new WaitForSecondsRealtime(windupTime);

        state.SetHostile(true);

        if (player && Vector2.Distance(transform.position, player.position) <= triggerDistance + 0.2f)
        {
            player.GetComponent<Health>()?.TakeDamage(damage);
            player.GetComponent<KnockbackReceiver>()?
                .ApplyKnockback(player.position - transform.position, knockbackForce);
        }
    }
}
