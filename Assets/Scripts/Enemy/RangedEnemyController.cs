using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RangedEnemyController : MonoBehaviour
{
    #region Declare Variables

    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [SerializeField] private float health;
    [SerializeField] private float colliderDistance;
    [SerializeField] private BoxCollider2D boxCollider;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;

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
            if (cooldownTimer >= attackCooldown)
            {
                Attack();
            }
        }

        if (enemyPatrol != null)
            enemyPatrol.enabled = !PlayerDetected();
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

    #endregion Unity Messages

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }



    #region Enemy Behaviours

    public void Hurt(int damage)
    {
        if (health < 0)
        {
            return;
        }
        health -= damage;
        if (health > 0)
        {
            StartCoroutine(Damaged());
        }
        else
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Damaged()
    {
        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;

        }
        animator.SetTrigger("Hurt");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = true;
        }
    }

    private IEnumerator Death()
    {

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
        }
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (enemyPatrol != null)
        {
            Destroy(enemyPatrol);
        }
        DestroyImmediate(gameObject);
    }

    private void Attack()
    {
        cooldownTimer = 0;
        animator.SetTrigger("Attack");
    }

    private bool PlayerDetected()
    {
        RaycastHit2D hit =
            Physics2D.BoxCast(boxCollider.bounds.center + transform.right * range * transform.localScale.x * colliderDistance,
            new Vector3(boxCollider.bounds.size.x * range, boxCollider.bounds.size.y, boxCollider.bounds.size.z),
            0, Vector2.left, 0, playerLayer);

        bool result = hit.collider != null;

        if (result)
        {
            player = hit.transform.GetComponent<KnightController>();
        }

        return result;
    }

    private void DamagePlayer()
    {
        if (PlayerDetected())
            player.Hurt(transform.position, damage);
    }

    #endregion Enemy Behaviours
}
