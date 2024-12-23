using UnityEngine;

public class BackgroundFollow : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera's transform

    private Vector3 offset; // The distance between the background and the camera

    void Start()
    {
        // Automatically assign the main camera if not set
        if (cameraTransform == null)
        {
            cameraTransform = Camera.main.transform;
        }

        // Calculate the initial offset
        offset = transform.position - cameraTransform.position;
    }

    void LateUpdate()
    {
        // Update the background position to follow the camera
        transform.position = cameraTransform.position + offset;
    }
}
