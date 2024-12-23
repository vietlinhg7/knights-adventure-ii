using UnityEngine;

public class CharacterMovementBounds : MonoBehaviour
{
    public Collider2D confinerCollider; // Assign the confiner collider

    private Vector2 boundsMin;
    private Vector2 boundsMax;

    void Start()
    {
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

    void Update()
    {
        // Get the current position
        Vector3 position = transform.position;

        // Clamp the position within the bounds
        position.x = Mathf.Clamp(position.x, boundsMin.x, boundsMax.x);
        position.y = Mathf.Clamp(position.y, boundsMin.y, boundsMax.y);

        // Apply the clamped position
        transform.position = position;
    }
}
