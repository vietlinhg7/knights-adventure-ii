using UnityEngine;

public class Dummy : MonoBehaviour
{
    [SerializeField]
    private int damage = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Get all components on the colliding GameObject
        Component[] components = collision.gameObject.GetComponents<Component>();

        // Loop through components to find a method named 'Hurt'
        foreach (Component component in components)
        {
            // Use reflection to check if the 'Hurt' method exists
            var method = component.GetType().GetMethod("Hurt");

            if (method != null)
            {
                // Call the 'Hurt' method with the dummy's position as the argument
                method.Invoke(component, new object[] { (Vector2)transform.position, damage });
                break; // Stop after finding the first valid 'Hurt' method
            }
        }
    }
}
