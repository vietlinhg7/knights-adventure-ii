using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemyController : MonoBehaviour
{
    #region Declare Variables

    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [SerializeField] private float health;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;

    private AudioSource audioSource;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;

    private KnightController player;
    private EnemyPatrol enemyPatrol;
    private float cooldownTimer = Mathf.Infinity;

    public Animator animator;

    #endregion Declare Variables

    #region Unity Messages

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        enemyPatrol = GetComponentInParent<EnemyPatrol>();
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        if (PlayerDetected())
        {
            Debug.Log("Attack");
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("Attack");
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerDetected();
    }

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.name.Contains("AttackArea"))
        {
            Debug.Log(collision.gameObject.transform);
            rigidbody2d.AddForce(new Vector2(MathF.Sign(gameObject.transform.position.x - collision.gameObject.transform.position.x), 1) * 2.5f, ForceMode2D.Impulse);
            StartCoroutine(Hurt());
        }
    }

    #endregion Unity Messages

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



    #region Enemy Behaviours

    private IEnumerator Hurt()
    {
        animator.SetTrigger("Hurt");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
    }

    private IEnumerator Death()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        DestroyImmediate(gameObject);
    }

    private bool PlayerDetected()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, groundLayer);

        if (hit.collider != null)
            player = hit.transform.GetComponent<KnightController>();

        return hit.collider != null;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (boxCollider != null)
        {
            Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
                new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z));
        }
    }

    private void DamagePlayer()
    {
        if (PlayerDetected())
            player.Hurt(transform.position, damage);
    }

    #endregion Enemy Behaviours
}
