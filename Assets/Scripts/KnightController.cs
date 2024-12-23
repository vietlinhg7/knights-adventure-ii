using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Drawing;
using Color = UnityEngine.Color;

public class KnightController : MonoBehaviour
{
    // Public Constants and Configurations
    public float timeInvincible = 2.0f;
    public float speed = 2.5f;
    public float jumpForce = 2.5f;
    public int maxHealth = 5;
    public float dashingPower = 24f;
    public float dashingTime = 1f;
    public float dashingCooldown = 1f;
    public float attackRange = 0.5f;
    public float attackRange2 = 1f;
    public float knockbackForce = 10f;
    public float knockbackUpwardForce = 5f;
    public float stunDuration = 0.5f;
    public float changeCooldown = 2f;
    public float attackTime = 0.22f;
    public float attackTime2 = 0.8f;
    public float attackTime3 = 3f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float attackMultiplier;
    public ChargingBarController chargingBarController;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private int[] characterAttack = { 1, 1, 1 }; //Attack for each class

    // Public State Variables
    public int health;
    public int characterClass = 0;

    // Components and References
    private Rigidbody2D rigidbody2d;
    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;
    public Animator knightAnimator;
    public Animator archerAnimator;
    public Animator wizardAnimator;
    public Animator animator;
    public Transform attackPoint;
    public Transform attackPoint2;
    public AudioSource audioSource;

    // Combat Areas and Layers
    public LayerMask enemyLayer;

    // Input and Movement
    float horizontal;
    float vertical;
    Vector2 lookDirection;

    // Dashing and Combat States
    private bool canDash = true;
    private bool isDashing = false;
    private bool attacking = false;
    private bool isStunned = false;
    private bool dead = false;


    // Invincibility
    bool isInvincible;
    float invincibleTimer;

    // Class Changing
    private bool canChangeClass = true;
    private bool isChanging = false;

    // Charge and Cooldowns
    private float chargeTime;

    // Visual Feedback
    private Color originalColor;
    private Color flashColor = Color.white;
    private Shader shaderGUItext;
    private Shader shaderSpritesDefault;

    // Call this function with the position of the damage dealer
    public void Hurt(Vector2 damageDealerPosition, int damage)
    {
        if (isStunned) return; // Prevent applying knockback if already stunned

        // Calculate the direction from the damage dealer to the player
        Vector2 knockbackDirection = ((Vector2)transform.position - damageDealerPosition).normalized;

        // Apply knockback force to the player
        Vector2 knockbackVector = new Vector2(knockbackDirection.x * knockbackForce, knockbackUpwardForce);
        rigidbody2d.linearVelocity = knockbackVector; // Override velocity for immediate response

        // Start the stun process
        health = health - damage;
        if (health > 0)
        {

            StartCoroutine(StunCoroutine());
            StartCoroutine(InvulnerableCoroutine(timeInvincible));
        }
        else
        {
            StartCoroutine(StunCoroutine());
        }
    }
    private IEnumerator InvulnerableCoroutine(float timeInvincible)
    {
        // Temporarily disable collision with other layers except the Tilemap
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dash");

        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        float elapsedTime = 0f;
        float blinkInterval = 0.1f; // Adjust the blink interval as needed

        while (elapsedTime < timeInvincible)
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // Toggle visibility
            }
            yield return new WaitForSeconds(blinkInterval);
            elapsedTime += blinkInterval;
        }

        // Ensure the sprite is visible when the invincibility ends
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = true;
        }

        // Revert the layer back to the original
        gameObject.layer = originalLayer;
    }

    private IEnumerator StunCoroutine()
    {
        isStunned = true;
        animator.SetTrigger("jump");

        // Wait for the stun duration
        yield return new WaitForSeconds(stunDuration);

        // Wait until the player is grounded
        while (!isGrounded())
        {
            yield return null; // Wait for the next frame
        }
        if (isGrounded())
        {
            rigidbody2d.linearVelocity = new Vector2(0f, 0f);
        }
        isStunned = false;

        // Check if the player is dead
        if (health <= 0)
        {
            dead = true;
            animator.SetBool("dead", true);
            animator.SetTrigger("die");
        }
    }


    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
            return;
        // Set the Gizmo color
        Gizmos.color = Color.red;

        // Draw a wire sphere to represent the attack range
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        // Set the Gizmo color
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(attackPoint2.position, attackRange2);
    }
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
    private Animator GetAnimatorByCharacterClass()
    {
        if (characterClass == 0)
            return knightAnimator;
        else if (characterClass == 1)
            return archerAnimator;
        else return wizardAnimator;
    }
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        /* currentHealth = maxHealth*/
        ;
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        boxCollider = GetComponent<BoxCollider2D>();
        tr = GetComponent<TrailRenderer>();
        health = maxHealth;
        animator.runtimeAnimatorController = GetAnimatorByCharacterClass().runtimeAnimatorController;
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
        chargingBarController.ShowBar(false);
    }

    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    // Update is called once per frame
    void Update()
    {
        if (dead || isChanging) return;
        animator.SetFloat("yVelocity", rigidbody2d.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded());
        if (isStunned) return;
        animator.SetBool("attacking", attacking);
        if (isDashing)
        {
            return;
        }
        horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0.01f)
            transform.localScale = Vector3.one;
        else if (horizontal < -0.01f)
            transform.localScale = new Vector3(-1, 1, 1);
        if (attacking)
        {
            return;
        }
        if (isInvincible)
        {
            invincibleTimer -= Time.deltaTime;
            if (invincibleTimer < 0)
                isInvincible = false;
        }
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded())
        {
            StartCoroutine(Attack());
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift) && canDash)
        {
            StartCoroutine(Dash());
        }
        else if (Input.GetKeyDown(KeyCode.X) && isGrounded())
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.F) && isGrounded())
        {
            if (!canChangeClass) return; // Prevent class change if cooldown is active

            StartCoroutine(ChangeClassRoutine());
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
    }
    private IEnumerator ChangeClassRoutine()
    {
        canChangeClass = false;

        // Flash the character white
        StartCoroutine(FlashWhite());

        // Change the character class
        characterClass = (characterClass + 1) % 3;

        // Wait for cooldown duration
        yield return new WaitForSeconds(changeCooldown);

        canChangeClass = true;
    }

    private IEnumerator FlashWhite()
    {
        isChanging = true;
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dash");

        originalColor = spriteRenderer.color;
        // Flash the character white for a short duration
        for (int i = 0; i < 3; i++) // Adjust the number of flashes as needed
        {
            spriteRenderer.material.shader = shaderGUItext;
            yield return new WaitForSeconds(0.05f);
            if (i == 1)
            {
                animator.runtimeAnimatorController = GetAnimatorByCharacterClass().runtimeAnimatorController;
            }
            spriteRenderer.material.shader = shaderSpritesDefault;
            yield return new WaitForSeconds(0.05f);
        }
        isChanging = false;
        // Revert the layer back to the original
        gameObject.layer = originalLayer;
    }
    private void FixedUpdate()
    {
        if (dead || isChanging) return;
        if (isStunned) return;
        if (isDashing)
        {
            return;
        }
        if (attacking)
        {
            return;
        }
        rigidbody2d.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed, rigidbody2d.linearVelocity.y);
    }
    private void Jump()
    {
        if (isGrounded())
        {
            rigidbody2d.linearVelocity = Vector2.up * jumpForce;
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
        // Temporarily disable collision with other layers except the Tilemap
        int originalLayer = gameObject.layer;
        gameObject.layer = LayerMask.NameToLayer("Dash");

        canDash = false;
        isDashing = true;
        animator.SetBool("dashing", true);
        animator.SetTrigger("dash");
        rigidbody2d.linearVelocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;

        float dashEndTime = Time.time + dashingTime;
        float afterImageInterval = 0.1f; // Interval between afterimages
        float nextAfterImageTime = 0f;
        Color color = spriteRenderer.color;
        Color oldColor = color;
        color.a = Mathf.Clamp(0.7f, 0f, 1f);
        spriteRenderer.color = color;

        while (Time.time < dashEndTime || isGrounded() == false)
        {
            if (Time.time >= nextAfterImageTime)
            {
                CreateAfterImage(); // Create afterimage
                nextAfterImageTime = Time.time + afterImageInterval;
            }

            yield return null;
        }

        // Revert the layer back to the original
        gameObject.layer = originalLayer;
        spriteRenderer.color = oldColor;

        tr.emitting = false;
        isDashing = false;
        animator.SetBool("dashing", false);
        animator.SetBool("isGrounded", isGrounded());

        // Wait for the cooldown
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }

    private void CreateAfterImage()
    {
        // Assuming you have a prefab for the afterimage
        GameObject afterImage = ObjectPooler.Instance.GetPooledObject("AfterImage"); // Object pool
        if (afterImage != null)
        {
            // Set the afterimage's position, rotation, and scale to match the player
            afterImage.transform.position = transform.position;
            afterImage.transform.rotation = transform.rotation;
            afterImage.transform.localScale = transform.localScale;

            SpriteRenderer afterImageSR = afterImage.GetComponent<SpriteRenderer>();
            SpriteRenderer playerSR = GetComponent<SpriteRenderer>();

            if (afterImageSR != null && playerSR != null)
            {
                // Copy the sprite and color of the player
                afterImageSR.sprite = playerSR.sprite;
                afterImageSR.color = playerSR.color;
            }

            // Activate the afterimage
            afterImage.SetActive(true);
        }
    }





    private IEnumerator Attack()
    {
        if (characterClass == 0)
        {
            int count = 0;
            do
            {
                if (count % 2 == 0)
                {
                    attacking = true;
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint2.position, attackRange2, enemyLayer);

                    foreach (Collider2D enemy in hitEnemies)
                    {
                        Debug.Log(this.name + " hit " + enemy.name);
                        enemy.GetComponent<EnemyController>().Hurt(characterAttack[characterClass]);
                    }
                    yield return new WaitForSeconds(attackTime);
                    rigidbody2d.linearVelocity = new Vector2(0f, 0f);
                }
                else
                {
                    attacking = true;
                    Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

                    foreach (Collider2D enemy in hitEnemies)
                    {
                        Debug.Log(this.name + " hit " + enemy.name); 
                        enemy.GetComponent<EnemyController>().Hurt(characterAttack[characterClass]);

                    }
                    yield return new WaitForSeconds(attackTime);
                    rigidbody2d.linearVelocity = new Vector2(0f, 0f);
                }
                count = count + 1;
            } while (Input.GetKey(KeyCode.Z) && isGrounded());
            attacking = false;
        }
        else if (characterClass == 1)
        {
            attacking = true;
            chargeTime = 0f;
            rigidbody2d.linearVelocity = Vector2.zero;
            animator.SetBool("isCharging", true);

            // Show the charging bar
            chargingBarController.ShowBar(true);
            chargingBarController.SetBarColor(Color.clear, Color.yellow);
            // Start charging while holding the key
            while (Input.GetKey(KeyCode.Z) && isGrounded())
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Min(chargeTime, maxChargeTime); // Clamp charge time

                if (chargeTime < attackTime2)
                    chargingBarController.SetChargeLevel(chargeTime, attackTime2);
                else
                {
                    chargingBarController.SetBarColor(Color.yellow, Color.red);
                    chargingBarController.SetChargeLevel(chargeTime - attackTime2, maxChargeTime - attackTime2);
                }
                print(chargeTime);
                yield return null;
            }
            chargingBarController.ShowBar(false);
            if (chargeTime < attackTime2)
            {
                animator.SetBool("isCharging", false);
            }
            else
            {
                // Calculate attack properties based on charge time
                if (chargeTime<maxChargeTime) attackMultiplier = 1f;
                else attackMultiplier = 2f; // E.g., up to 2x attack power

                // Instantiate the arrow
                GameObject arrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

                // Calculate launch direction and force
                Vector3 playerPosition = new Vector3(transform.position.x, transform.position.y, 0); // Replace with your player's position
                Vector3 launchDirection = arrowSpawnPoint.position - playerPosition; // Calculate the direction from arrowSpawnPoint to player
                launchDirection = new Vector3(launchDirection.x, 0, 0);
                launchDirection.Normalize(); // Ensure the direction is normalized

                // Pass the launch force to the arrow
                ArrowController arrowController = arrow.GetComponent<ArrowController>();
                if (arrowController != null)
                {
                    arrowController.launchDirection = launchDirection;
                    arrowController.launchSpeed = 100f * attackMultiplier ;
                }

                // Trigger the release animation
                animator.SetTrigger("release");
            }

            // Reset states
            chargeTime = 0f;
            attackMultiplier = 1f;
            attacking = false;
        }
        else if (characterClass == 2)
        {
            attacking = true;
            chargeTime = 0f;
            rigidbody2d.linearVelocity = Vector2.zero;
            animator.SetBool("isCharging", true);

            // Start charging while holding the key
            while (Input.GetKey(KeyCode.Z) && isGrounded() && chargeTime<attackTime3)
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Min(chargeTime, attackTime3); // Clamp charge time
                print(chargeTime);
                yield return null;
            }
            animator.SetBool("isCharging", false);
            if (chargeTime == attackTime3) {
                
            }
            // Reset states
            chargeTime = 0f;
            attacking = false;
        }
    }
}
