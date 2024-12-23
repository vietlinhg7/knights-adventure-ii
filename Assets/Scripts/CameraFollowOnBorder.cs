using UnityEngine;

public class CameraFollowOnBorder : MonoBehaviour
{
    public Transform characterTransform; // The character to follow
    public float borderThreshold = 0.1f; // The percentage of screen space from edges to start following
    public float followSpeed = 5f; // Speed of the camera following the character
    private Camera mainCamera;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        // Get the main camera or assign a custom camera here
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Convert character position to screen space
        Vector3 characterScreenPos = mainCamera.WorldToScreenPoint(characterTransform.position);

        // Get screen boundaries
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Check if the character is near the border of the screen
        bool isNearBorder =
            characterScreenPos.x < borderThreshold * screenWidth ||
            characterScreenPos.x > (1 - borderThreshold) * screenWidth ||
            characterScreenPos.y < borderThreshold * screenHeight ||
            characterScreenPos.y > (1 - borderThreshold) * screenHeight;

        // Debugging: Check character screen position and border status
        Debug.Log("Character Screen Position: " + characterScreenPos);
        Debug.Log("Is Near Border: " + isNearBorder);

        // If the character is near the border, move the camera to follow it
        if (isNearBorder)
        {
            Vector3 targetPosition = new Vector3(characterTransform.position.x, characterTransform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, 0.3f);  // 0.3f is the smooth time
        }
    }
}
