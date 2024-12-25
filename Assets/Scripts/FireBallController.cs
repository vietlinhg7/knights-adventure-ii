using UnityEngine;

public class FireBallController : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    private Transform target;
    private Rigidbody2D rb;
    private BoxCollider2D boxCollider2D;

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        boxCollider2D = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {

        Debug.Log("I hit " + collision.gameObject.name);
        if (collision.gameObject.GetComponent<EnemyController>())
        {
            collision.gameObject.GetComponent<EnemyController>().Hurt(damage);
        }
        Destroy(boxCollider2D.gameObject);

    }
}
