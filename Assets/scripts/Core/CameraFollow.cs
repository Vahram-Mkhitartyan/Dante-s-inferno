using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public Vector2 screenOffset = new Vector2(6f, 0.5f);
    public float smoothSpeed = 6f;

    void LateUpdate()
    {
        if (!target) return;

        Vector3 desiredPosition = new Vector3(
            target.position.x + screenOffset.x,
            target.position.y + screenOffset.y,
            -10f
        );

        float maxMovePerFrame = smoothSpeed * Time.deltaTime;

        transform.position = Vector3.MoveTowards(
            transform.position,
            desiredPosition,
            maxMovePerFrame
        );
    }

}
