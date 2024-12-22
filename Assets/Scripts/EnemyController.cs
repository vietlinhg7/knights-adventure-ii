using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.VisualScripting.ReorderableList;

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

    private AudioSource audioSource;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    private float hurtDuration = 1;

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Health health;
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
    public float range = 1f;
    public float jumpForce = 2.5f;

    private Vector2 rootPosition = new Vector2();
    private bool iframe = false;

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
        StartCoroutine(Moving());
    }

    private void OnEnable()
    {
        enemyState = EnemyState.Moving;
    }

    //private void Update() // Change to coroutine to reduce update call
    //{
    //}

    private void OnCollisionEnter2D(UnityEngine.Collision2D collision)
    {
        if (collision.gameObject.name.Contains("AttackArea") && !iframe)
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

    private IEnumerator Moving()
    {
        animator.SetTrigger("Move");
        rootPosition = gameObject.transform.position;
        while (enemyState == EnemyState.Moving)
        {
            Vector2 position = rigidbody2d.position;
            position.x = rootPosition.x + range * MathF.Sin(Time.fixedTime);
            rigidbody2d.MovePosition(position);
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator Hurt()
    {
        iframe = true;
        //health.Damage(1);
        Debug.Log("Hurt");

        if (false) //Health = 0
        {
            yield return Death();
        }
        else
        {
            animator.SetTrigger("Hurt");
            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
            StartCoroutine(Moving());
        }

        iframe = false;
    }

    private IEnumerator Death()
    {
        animator.SetTrigger("Death");
        yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);
        DestroyImmediate(gameObject);
    }

    #endregion Enemy Behaviours
}
