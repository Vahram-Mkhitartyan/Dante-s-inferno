using UnityEngine;
using System.Collections;

public class PlayerRespawnController : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnDelay = 1.5f;
    public Transform respawnPoint;

    private Health health;
    private PlayerController controller;
    private PlayerSpineAnimationController anim;

    private Vector3 startPosition;

    void Awake()
    {
        health = GetComponent<Health>();
        controller = GetComponent<PlayerController>();
        anim = GetComponentInChildren<PlayerSpineAnimationController>();

        startPosition = transform.position;

        if (!respawnPoint)
            respawnPoint = transform; // fallback

        health.OnDeath += HandleDeath;
    }

    void HandleDeath()
    {
        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        // Wait for death animation to be seen
        yield return new WaitForSeconds(respawnDelay);

        // --- RESET POSITION ---
        transform.position = respawnPoint.position;

        // --- RESET HEALTH ---
        health.ResetHealth();

        // --- RESET CONTROLLER ---
        controller.SetLocked(false);

        // --- RESET ANIMATION ---
        anim.ResetToIdle();
    }
}
