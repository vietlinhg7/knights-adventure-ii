using Unity.VisualScripting;
using UnityEngine;
using System.Collections;

public class EnemyRangedAttackController : MonoBehaviour
{
    public Vector3 launchDirection;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider;
    [SerializeField] private int damage = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();

        rb.AddForce(launchDirection);
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        rb.linearVelocity = Vector3.zero;
        rb.bodyType = RigidbodyType2D.Static;

        Debug.Log("I hit " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<KnightController>())
        {
            collision.gameObject.GetComponent<KnightController>().Hurt(transform.position, damage);
            Destroy(rb.gameObject);
        }

        transform.SetParent(collision.gameObject.transform);
        boxCollider.enabled = false;
    }
}
