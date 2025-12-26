using UnityEngine;

public class RowingArmMotion : MonoBehaviour
{
    [Header("References")]
    public Transform upperArm;     // Shoulder joint
    public Transform lowerArm;     // Elbow / forearm

    [Header("Timing")]
    public float startDelay = 1.2f;   // Delay before rowing begins
    public float speed = 1.2f;        // Overall rowing speed

    [Header("Rotation")]
    public float shoulderAmplitude = 25f;
    public float elbowAmplitude = 35f;

    [Header("Motion Polish")]
    public float elbowLiftAmount = 0.08f;   // Vertical compensation
    public float phaseOffset = 0.6f;        // Delay between shoulder & elbow

    private Vector3 lowerArmStartLocalPos;
    private float startTime;
    private bool started = false;

    void Start()
    {
        if (lowerArm != null)
            lowerArmStartLocalPos = lowerArm.localPosition;

        startTime = Time.time;
    }

    void Update()
    {
        // Wait for start delay
        if (!started)
        {
            if (Time.time >= startTime + startDelay)
                started = true;
            else
                return;
        }

        float t = (Time.time - (startTime + startDelay)) * speed;

        // --- SHOULDER MOTION ---
        float shoulderAngle = Mathf.Sin(t) * shoulderAmplitude;
        upperArm.localRotation = Quaternion.Euler(0f, 0f, shoulderAngle);

        // --- FOREARM MOTION (biased downward) ---
        float raw = Mathf.Sin(t + phaseOffset);
        float biased = Mathf.Lerp(-1f, raw, 0.6f); // downward weighted
        float elbowAngle = biased * elbowAmplitude;

        lowerArm.localRotation = Quaternion.Euler(0f, 0f, elbowAngle);

        // --- Vertical lift to simulate muscle movement ---
        float lift = Mathf.Abs(biased) * elbowLiftAmount;
        lowerArm.localPosition = lowerArmStartLocalPos + new Vector3(0f, lift, 0f);
    }
}
