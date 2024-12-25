using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
public class FireBallController : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float explosionRadius = 1f; // Radius of the explosion
    private Transform target;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;
    private CircleCollider2D explosionCollider;
    private SpriteRenderer spriteRenderer; // Reference to the sprite renderer
    public bool explode = false;
    public LayerMask enemyLayer;
    private Vector2 direction; // Direction to move toward the target
    public AudioSource explodehit;
    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        rb = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        explosionCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Ensure the explosion collider is disabled initially
        if (explosionCollider != null)
        {
            explosionCollider.enabled = false;
            explosionCollider.radius = explosionRadius; // Set the collider radius
        }

        // Set the initial direction toward the target
        if (target != null)
        {
            direction = (target.position - transform.position).normalized;
        }
    }

    void Update()
    {
        if (direction != Vector2.zero)
        {
            transform.position += (Vector3)direction * speed * Time.deltaTime;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("I hit " + collision.gameObject.name);
        if (explode)
        {
            // Scale the sprite based on the explosion radius
            ScaleSpriteToRadius();
            StartCoroutine(Explode());
        }
        else
        {
            if (collision.gameObject.GetComponent<EnemyController>())
            {
                collision.gameObject.GetComponent<EnemyController>().Hurt(damage);
            }
            Destroy(gameObject);
        }
    }

    private IEnumerator Explode()
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetTrigger("explode");

        }

        // Disable movement and the box collider
        speed = 0f;
        if (boxCollider2D != null)
        {
            boxCollider2D.enabled = false;
        }

        // Wait for the explosion animation to start
        yield return new WaitForSeconds(0.3f);

        // Enable the explosion collider
        if (explosionCollider != null && explosionCollider.enabled == false)
        {
            explosionCollider.enabled = true;
            if (explodehit != null)
            {
                explodehit.Play();
            }
            // Optionally damage all enemies within the explosion radius
            Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(transform.position, explosionCollider.radius, enemyLayer);
            foreach (Collider2D hit in hitEnemies)
            {
                if (hit.gameObject.GetComponent<EnemyController>())
                {
                    hit.GetComponent<EnemyController>()?.Hurt(damage);
                }
            }
        }

        // Wait for the explosion animation to complete
        yield return new WaitForSeconds(0.383f);

        // Destroy the fireball GameObject
        Destroy(gameObject);
    }

    private void ScaleSpriteToRadius()
    {
        if (spriteRenderer != null)
        {
            // Calculate the required scale factor based on the collider's radius and the sprite's size
            float diameter = explosionRadius;
            Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
            float scaleX = diameter;
            float scaleY = diameter;
            // Apply the scale to the transform
            transform.localScale = new Vector3(scaleX, scaleY, 1f);
        }
    }
}
