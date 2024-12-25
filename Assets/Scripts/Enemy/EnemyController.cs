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
    [SerializeField] private EnemyHealthBar healthBar;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float noiseRange = 4f; // Distance at which noise plays
    private bool isPlayerNearby = false;
    //Sound effect
    public AudioSource hit;
    public AudioSource dead;
    public AudioSource noise;
    public AudioSource attack;

    private AudioSource audioSource;
    private Rigidbody2D rigidbody2d;
    private SpriteRenderer spriteRenderer;

    private KnightController player;
    public EnemyPatrol enemyPatrol;
    private float cooldownTimer = Mathf.Infinity;

    public Animator animator;
    private float maxHealth;
    private bool allowedToMove = true;

    #endregion Declare Variables

    #region Unity Messages

    void Start()
    {
        maxHealth = health;
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
        CheckForPlayerProximity();
        if (PlayerDetected())
        {
            if (cooldownTimer >= attackCooldown)
            {
                Attack();
            }
        }

        if (enemyPatrol != null && allowedToMove)
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
        healthBar.UpdateHealth(health / maxHealth);

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = false;
            allowedToMove = false;
        }


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
        animator.SetTrigger("Hurt");
        if (hit != null)
        {
            hit.Play();
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

        if (enemyPatrol != null)
        {
            enemyPatrol.enabled = true;
            allowedToMove = true;
        }
    }

    private IEnumerator Death()
    {
        DropCoin();
        animator.SetTrigger("Death");
        if (dead != null)
        {
            dead.Play();
        }

        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        if (enemyPatrol != null)
        {
            Destroy(enemyPatrol.gameObject);
        }
        DestroyImmediate(gameObject);
    }

    private void DropCoin()
    {
        FindFirstObjectByType<KnightController>().AddCoin();
        GameObject coin = Instantiate(coinPrefab);
        coin.transform.SetParent(gameObject.transform, false);
    }

    protected virtual void Attack()
    {
        cooldownTimer = 0;
        if(attack != null)
        {
            attack.Play();
        }

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

    #region Proximity Noise

    private void CheckForPlayerProximity()
    {
        if (player == null)
        {
            return;
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log("Player detected: " + distanceToPlayer);  // Debugging the player detection
        Console.WriteLine("Distance to player: " + distanceToPlayer);  // Debugging the distance

        // Check if the player is within range to hear the slime
        if (distanceToPlayer <= noiseRange)
        {
            if (!isPlayerNearby)
            {
                // Play noise sound if the player enters range
                noise.Play();
                isPlayerNearby = true;
            }
        }
        else
        {
            if (isPlayerNearby)
            {
                // Stop noise sound if the player moves out of range
                noise.Stop();
                isPlayerNearby = false;
            }
        }
    }


    #endregion Proximity Noise
}
