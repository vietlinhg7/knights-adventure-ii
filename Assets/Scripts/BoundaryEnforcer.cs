using UnityEngine;

public class BackgroundFollowConfiner : MonoBehaviour
{
    public Transform cameraTransform; // Cinemachine camera or Main Camera
    public Collider2D confinerCollider; // The collider for confining camera and background
    public Vector3 offset; // Optional offset for background positioning

    private Vector2 boundsMin;
    private Vector2 boundsMax;

    void Start()
    {
        // Ensure that the confiner collider is assigned
        if (confinerCollider != null)
        {
            boundsMin = confinerCollider.bounds.min;
            boundsMax = confinerCollider.bounds.max;
        }
        else
        {
            Debug.LogError("Confiner Collider not assigned!");
        }
    }

    void LateUpdate()
    {
        if (cameraTransform == null || confinerCollider == null) return;

        // Get the target position based on the camera's position and offset
        Vector3 targetPosition = cameraTransform.position + offset;

        // Clamp the background position to stay within the boundaries
        float clampedX = Mathf.Clamp(targetPosition.x, boundsMin.x, boundsMax.x);
        float clampedY = Mathf.Clamp(targetPosition.y, boundsMin.y, boundsMax.y);

        // Apply the clamped position to the background
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }
}
