using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;        // The object the camera follows (e.g., the player)
    public Collider2D levelBounds;  // The collider defining the level boundaries (e.g., a BoxCollider2D)

    private Camera cam;

    void Start()
    {
        cam = Camera.main;
    }

    void LateUpdate()
    {
        if (target != null && levelBounds != null)
        {
            // Get the target's position (the object the camera follows)
            Vector3 targetPosition = target.position;

            // Camera's current position
            Vector3 cameraPosition = transform.position;

            // Set the camera's position based on the target's position (this can be the player or any other object)
            cameraPosition.x = targetPosition.x;
            cameraPosition.y = targetPosition.y;

            // Get the level's bounding box from the collider
            Bounds levelBoundsBox = levelBounds.bounds;

            // Get the camera's orthographic size and aspect ratio to calculate the camera's view width/height
            float cameraHeight = cam.orthographicSize * 2;
            float cameraWidth = cameraHeight * cam.aspect;

            // Clamp the camera's position within the level's bounds, considering the camera's view size
            cameraPosition.x = Mathf.Clamp(cameraPosition.x, levelBoundsBox.min.x + cameraWidth / 2, levelBoundsBox.max.x - cameraWidth / 2);
            cameraPosition.y = Mathf.Clamp(cameraPosition.y, levelBoundsBox.min.y + cameraHeight / 2, levelBoundsBox.max.y - cameraHeight / 2);

            // Apply the new clamped position
            transform.position = cameraPosition;
        }
    }
}
