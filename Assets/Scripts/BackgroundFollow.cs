using UnityEngine;

public class BackgroundFollowBounds : MonoBehaviour
{
    public Transform cameraTransform; // Assign the Cinemachine virtual camera or Main Camera
    public Vector3 offset;            // Offset to adjust the background position

    void LateUpdate()
    {
        if (cameraTransform == null) return;

        // Set the background position based on the camera position + offset
        transform.position = new Vector3(cameraTransform.position.x + offset.x,
                                         cameraTransform.position.y + offset.y,
                                         transform.position.z + offset.z);
    }
}
