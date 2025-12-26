using UnityEngine;
using System.Collections;

public class RowingBoatMovement : MonoBehaviour
{
    [Header("Rowing")]
    public float rowInterval = 3f;        // seconds between strokes
    public float strokeDuration = 0.3f;   // how long the push lasts
    public float strokeForce = 8f;         // acceleration magnitude
    public float riverSpeed = 0.4f; // constant drift
    [Header("Drag")]
    public float quadraticDrag = 1.2f;     // water resistance
    public float linearDrag = 0.3f;        // settling drag

    [Header("Direction")]
    public Vector2 moveDirection = Vector2.right;

    private float velocity;
    //private bool rowing;
    

    void Start()
    {
        moveDirection.Normalize();
        StartCoroutine(RowRoutine());
    }

    void Update()
    {
        float dt = Time.deltaTime;

        //Fd = a(row)*t - kv|v| * t

        // Apply drag (always)
        transform.position += (Vector3)(moveDirection * (riverSpeed + velocity) * dt);
        velocity -= quadraticDrag * velocity * Mathf.Abs(velocity) * dt;
        velocity -= linearDrag * velocity * dt;

        // Move boat
        transform.position += (Vector3)(moveDirection * velocity * dt);
    }

    IEnumerator RowRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(rowInterval);
            yield return StartCoroutine(ApplyStroke());
        }
    }

    IEnumerator ApplyStroke()
    {
        //rowing = true;
        float t = 0f;

        while (t < strokeDuration)
        {
            float normalized = t / strokeDuration;
            float pulse = Mathf.Sin(normalized * Mathf.PI); // smooth in/out
            velocity += strokeForce * pulse * Time.deltaTime;

            t += Time.deltaTime;
            yield return null;
        }

        //rowing = false;
    }
}
