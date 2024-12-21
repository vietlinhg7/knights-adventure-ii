using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class KnightController : MonoBehaviour
{
    //public float timeInvincible = 2.0f;
    //bool isInvincible;
    //float invincibleTimer;
    public float speed = 2.5f;
    public float jumpForce = 2.5f;
    //public int maxHealth = 5;
    //public int health { get { return currentHealth; } }
    //public int currentHealth;
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider;
    float horizontal;
    float vertical;
    Vector2 lookDirection;
    public Animator animator;
    //public GameObject projectilePrefab;
    AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private LayerMask groundLayer;
    private bool canDash = true;
    private bool isDashing;
    public float dashingPower = 24f;
    public float dashingTime = 1;
    public float dashingCooldown = 1f;
    [SerializeField] private TrailRenderer tr;
    //void Launch()
    //{
    //    GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f,

    //    Quaternion.identity);

    //    Projectile projectile = projectileObject.GetComponent<Projectile>();
    //    projectile.Launch(lookDirection, 300);
    //    animator.SetTrigger("Launch");
    //}
    //public void ChangeHealth(int amount)
    //{
    //    if (amount < 0)
    //    {
    //        animator.SetTrigger("Hit");
    //        if (isInvincible) return;
    //        isInvincible = true; invincibleTimer = timeInvincible;
    //    }
    //    currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    //    //UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    //}
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
       /* currentHealth = maxHealth*/;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        tr = GetComponent<TrailRenderer>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isGrounded", isGrounded());
        if (isDashing)
        {
            return;
        }
        horizontal = Input.GetAxis("Horizontal");
        //if (isInvincible)
        //{
        //    invincibleTimer -= Time.deltaTime;
        //    if (invincibleTimer < 0)
        //        isInvincible = false;
        //}
        if (horizontal > 0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (horizontal < -0.01f)
            transform.localScale = Vector3.one;
        if (Input.GetKey(KeyCode.S) && isGrounded())
        {
            Jump();
            animator.SetTrigger("jump");
        }
        if (Input.GetKey(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        //if (Input.GetKeyDown(KeyCode.Space))
        //{
        //    Launch();
        //}
        //if (Input.GetKeyDown(KeyCode.X))
        //{
        //    RaycastHit2D hit = Physics2D.Raycast(rigidbody2d.position + Vector2.up * 0.2f,
        //    lookDirection, 1.5f, LayerMask.GetMask("NPC"));
        //    if (hit.collider != null)
        //    {
        //        NonPlayerCharacter character =
        //        hit.collider.GetComponent<NonPlayerCharacter>();
        //        if (character != null)
        //        {
        //            character.DisplayDialog();
        //        }
        //    }
        //}
        //animator.SetFloat("Look X", lookDirection.x);
        //animator.SetFloat("Look Y", lookDirection.y);
        animator.SetBool("run", horizontal != 0);
        animator.SetFloat("yVelocity", rigidbody2d.linearVelocity.y);
    }
    private void FixedUpdate()
    {
        if (isDashing)
        {
            return;
        }
        rigidbody2d.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigidbody2d.linearVelocity.y);
    }
    private void Jump()
    {
        if (isGrounded())
        {
            rigidbody2d.linearVelocity = Vector2.up * jumpForce * 4 / 3;
            animator.SetTrigger("jump");
        }
        
    }
    private bool isGrounded()
    {
        // Adjust the size to be slightly smaller in width, ensuring it only checks below the character
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x, boxCollider.bounds.size.y);
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider.bounds.center, boxSize, 0, Vector2.down, 0.1f, groundLayer);
        return raycastHit.collider != null;
    }
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        animator.SetTrigger("dash");
        rigidbody2d.linearVelocity = new Vector2(-transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
