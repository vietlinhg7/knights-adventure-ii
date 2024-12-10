using UnityEngine;

public class KnightController : MonoBehaviour
{
    public float timeInvincible = 2.0f;
    bool isInvincible;
    float invincibleTimer;
    public float speed = 2.5f;
    public int maxHealth = 5;
    public int health { get { return currentHealth; } }
    public int currentHealth;
    Rigidbody2D rigidbody2d;
    float horizontal;
    float vertical;
    Vector2 lookDirection;
    public Animator animator;
    public GameObject projectilePrefab;
    AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    //void Launch()
    //{
    //    GameObject projectileObject = Instantiate(projectilePrefab, rigidbody2d.position + Vector2.up * 0.5f,

    //    Quaternion.identity);

    //    Projectile projectile = projectileObject.GetComponent<Projectile>();
    //    projectile.Launch(lookDirection, 300);
    //    animator.SetTrigger("Launch");
    //}
    public void ChangeHealth(int amount)
    {
        if (amount < 0)
        {
            animator.SetTrigger("Hit");
            if (isInvincible) return;
            isInvincible = true; invincibleTimer = timeInvincible;
        }
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
        //UIHealthBar.instance.SetValue(currentHealth / (float)maxHealth);
    }
    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        Vector2 move = new Vector2(horizontal, 0.0f);
        if (!Mathf.Approximately(move.x, 0.0f) || !Mathf.Approximately(move.y, 0.0f))
        {
            spriteRenderer.flipX = move.x < 0.0f;
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
        animator.SetFloat("Speed", move.magnitude);
    }
    private void FixedUpdate()
    {
        Vector2 position = transform.position;
        position.x = position.x + speed * horizontal * Time.deltaTime;
        position.y = position.y + speed * vertical * Time.deltaTime;
        rigidbody2d.MovePosition(position);
    }
}
