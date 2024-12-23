using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public enum EnemyState
{
    Idle,
    Moving,
    Hurt,
    Death
}

public class EnemyController : MonoBehaviour
{
    #region Declare Variables

    [SerializeField] private float attackCooldown;
    [SerializeField] private float range;
    [SerializeField] private int damage;

    [SerializeField] private Health health;
    [SerializeField] private float colliderDistance;
    [SerializeField] private LayerMask groundLayer;

    private AudioSource audioSource;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    private Health playerHealth;
    private EnemyPatrol enemyPatrol;
    private float cooldownTimer = Mathf.Infinity;

    private EnemyState enemyState = EnemyState.Idle;
    public EnemyState EnemyState
    {
        get { return enemyState; }
        set
        {
            this.enemyState = value;

            switch (enemyState)
            {
                case EnemyState.Idle:
                    break;
                case EnemyState.Moving:
                    break;
                case EnemyState.Hurt:
                    break;
                case EnemyState.Death:
                    break;
            }
        }
    }

    public Animator animator;

    private Vector2 rootPosition = new Vector2();

    #endregion Declare Variables

    #region Unity Messages

    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();

        rootPosition = gameObject.transform.position;
    }

    private void OnEnable()
    {
        enemyState = EnemyState.Moving;
    }

    private void Update()
    {
        cooldownTimer += Time.deltaTime;

        //Attack only when player in sight?
        if (PlayerDetected())
        {
            if (cooldownTimer >= attackCooldown)
            {
                cooldownTimer = 0;
                animator.SetTrigger("meleeAttack");
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
            playerHealth = hit.transform.GetComponent<Health>();

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
            playerHealth.Damage(damage);
    }

    #endregion Enemy Behaviours
}
