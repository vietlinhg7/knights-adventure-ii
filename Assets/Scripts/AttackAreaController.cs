using UnityEngine;

public class AttackAreaController : MonoBehaviour
{
    private int damage = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void OnEnable()
    {
        // Get all colliders overlapping the object's collider(s).
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, GetComponent<Collider2D>().bounds.size, 0f);

        // Loop through each collider to check for the Health component and apply damage.
        foreach (Collider2D collider in colliders)
        {
            if (collider != null && collider.GetComponent<Health>() != null)
            {
                Health health = collider.GetComponent<Health>();
                health.Damage(damage);
            }
        }
    }
}
