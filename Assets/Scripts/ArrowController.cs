using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class ArrowController : MonoBehaviour
{
    public Vector3 launchDirection;
    public float launchSpeed;
    private Rigidbody2D rb; // Reference to the Rigidbody component
    private BoxCollider2D boxCollider;
    [SerializeField] private int baseDamage = 1;
    public int damage;

    void Start()
    {
        gameObject.layer = LayerMask.NameToLayer("Arrow");
        // Get the Rigidbody component attached to the arrow
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        // Launch the arrow with the specified force vector
        rb.AddForce(launchSpeed * launchDirection);
        damage = (int)(launchSpeed / 100f) * baseDamage;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Stop the arrow when it collides with something
        rb.linearVelocity = Vector3.zero; // Set velocity to zero
        rb.bodyType = RigidbodyType2D.Static;  // Make the Rigidbody kinematic to prevent further physics interactions

        Debug.Log("I hit " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            collision.gameObject.GetComponent<EnemyController>().Hurt(damage);
            Destroy(rb.gameObject);
        }
        
        // Optionally, you can parent the arrow to the object it collided with
        transform.SetParent(collision.gameObject.transform);
        boxCollider.enabled = false;

    }
}
