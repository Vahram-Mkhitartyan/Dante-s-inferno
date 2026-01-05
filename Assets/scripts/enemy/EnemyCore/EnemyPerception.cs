using UnityEngine;

public class EnemyPerception : MonoBehaviour
{
    public float detectRange = 6f;
    public float forgetRange = 8f;

    private Transform player;
    private bool playerPassed = false;

    public bool PlayerPassed => playerPassed;

    void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    public bool CanSeePlayer()
    {
        if (!player) return false;
        return Vector2.Distance(transform.position, player.position) <= detectRange;
    }

    public bool HasPlayerPassed()
    {
        if (!player) return false;

        float dir = Mathf.Sign(player.position.x - transform.position.x);
        float facing = Mathf.Sign(transform.right.x);

        // player crossed to behind
        if (!playerPassed && dir != facing)
        {
            playerPassed = true;
        }

        return playerPassed;
    }

    public void ResetPass()
    {
        playerPassed = false;
    }
}
