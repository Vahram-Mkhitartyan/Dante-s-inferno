using UnityEngine;
using System.Collections;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnDelay = 1.5f;
    public float postRespawnInputDelay = 0.25f;
    public float cameraReturnDuration = 2f;
    public Transform respawnPoint;

    private Health health;
    private PlayerController controller;
    private PlayerSpineAnimationController anim;
    private Rigidbody2D rb;
    private CameraFollow cameraFollow;

    private Vector3 startPosition;

    void Awake()
    {
        health = GetComponent<Health>();
        controller = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerSpineAnimationController>();
        rb = GetComponent<Rigidbody2D>();
        cameraFollow = FindAnyObjectByType<CameraFollow>();

        startPosition = transform.position;

        if (health != null)
        {
            health.destroyOnDeath = false;
            health.OnDeath += HandleDeath;
        }
    }

    void OnDestroy()
    {
        if (health != null)
            health.OnDeath -= HandleDeath;
    }

    void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        Vector3 destination = respawnPoint != null ? respawnPoint.position : startPosition;
        cameraFollow?.MoveToTargetFraming(destination, cameraReturnDuration);

        // Wait for death animation to be seen
        yield return new WaitForSeconds(respawnDelay);

        // --- RESET POSITION ---
        transform.position = destination;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // --- RESET HEALTH ---
        health?.ResetHealth();

        // --- RESET CONTROLLER ---
        controller?.SetLocked(false);

        // --- RESET ANIMATION ---
        anim?.ResetToIdle();

        if (postRespawnInputDelay > 0f)
        {
            controller?.SetLocked(true);
            yield return new WaitForSeconds(postRespawnInputDelay);
            controller?.SetLocked(false);
        }
    }
}
