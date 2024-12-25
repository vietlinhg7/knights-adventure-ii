using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UIElements;
using System.Drawing;
using Color = UnityEngine.Color;
using Unity.VisualScripting;
using System.IO;
using UnityEngine.SceneManagement;

public class KnightController : MonoBehaviour
{
    // Public Constants and Configurations
    public float timeInvincible = 2.0f;
    public int wizardUpgrade = 1;
    public float speed = 2.5f;
    public float jumpForce = 2.5f;
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
    public int manaCost = 20;
    public int maxHealth = 100;
    public int maxMana = 100;
    public int maxArrows = 10;
    public int maxArmor = 100;
    public int currentCoin = 10;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private TrailRenderer tr;
    [SerializeField] private float maxChargeTime;
    [SerializeField] private float attackMultiplier;
    public ChargingBarController chargingBarController;
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowSpawnPoint;
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private Transform fireBallSpawnPoint;
    [SerializeField] private int[] characterAttack = { 1, 1, 1 }; //Attack for each class
    [SerializeField] private HUDController HUDController; //Attack for each class

    // Public State Variables
    public int characterClass = 0;
    public int health;
    public int mana;
    public int arrows = 10;
    public int armor = 100;
    public bool wrecked;

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
    public AudioSource jumpSound;  // Reference to AudioSource component
    public AudioSource BowSound;  // Reference to AudioSource component
    public AudioSource SwordSound;  // Reference to AudioSource component
    public AudioSource MagicAtk;  // Reference to AudioSource component
    public AudioSource MaleDead;  // Reference to AudioSource component
    public AudioSource Charging;  // Reference to AudioSource component
    public AudioSource Hit;  // Reference to AudioSource component
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
        if (characterClass == 0 && armor > 0) {
            armor = armor - damage;
            if (armor < 0)
                wrecked = true;
        }
        else {
            health = health - damage;
        }
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
        if (Hit != null)
        {
            Hit.Play();
        }
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
            if (MaleDead != null)
            {
                MaleDead.Play();
            }
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
        mana = maxMana;
        animator.runtimeAnimatorController = GetAnimatorByCharacterClass().runtimeAnimatorController;
        shaderGUItext = Shader.Find("GUI/Text Shader");
        shaderSpritesDefault = Shader.Find("Sprites/Default");
        chargingBarController.ShowBar(false);

        GameObject HUD = GameObject.FindWithTag("HUD");
        HUDController = HUD.GetComponent<HUDController>();
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
        if (Input.GetKeyDown(KeyCode.Z) && (isGrounded() || characterClass == 1))
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
            if (jumpSound != null)
            {
                if (!dead)
                    jumpSound.Play();
            }
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
                HUDController.changeClass(characterClass, changeCooldown);
            }
            spriteRenderer.material.shader = shaderSpritesDefault;
            yield return new WaitForSeconds(0.05f);
        }
        isChanging = false;
        // Revert the layer back to the original
        gameObject.layer = LayerMask.NameToLayer("Player");
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
        gameObject.layer = LayerMask.NameToLayer("Player");
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
                if(SwordSound != null)
                {
                    SwordSound.Play();
                }
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
            while (Input.GetKey(KeyCode.Z))
            {
                rigidbody2d.gravityScale = 0.2f;
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
            rigidbody2d.gravityScale = 2f;
            chargingBarController.ShowBar(false);
            if (chargeTime < attackTime2 || arrows == 0)
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
                if(BowSound != null)
                {
                    BowSound.Play();
                }
                // Trigger the release animation
                animator.SetTrigger("release");
                arrows = arrows - 1;
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

            // Instantiate the arrow
            GameObject fireBall = Instantiate(fireBallPrefab, fireBallSpawnPoint.position, fireBallSpawnPoint.rotation);
            FireBallController fireBallController = fireBall.GetComponent<FireBallController>();
            fireBallController.damage = 60 + (wizardUpgrade * 20) ;
            fireBallController.enemyLayer = enemyLayer;
            fireBallController.explosionRadius = (1 + (wizardUpgrade - 1) * 0.5f);
            fireBallController.explode = wizardUpgrade > 0;

            fireBallController.transform.SetParent(this.gameObject.transform);
            fireBallController.GetComponent<BoxCollider2D>().enabled = false;
            fireBallController.GetComponent<CircleCollider2D>().enabled = false;

            // Start charging while holding the key
            while (Input.GetKey(KeyCode.Z) && isGrounded() && chargeTime<attackTime3)
            {
                chargeTime += Time.deltaTime;
                chargeTime = Mathf.Min(chargeTime, attackTime3); // Clamp charge time
                                                                 // Play charging sound if it's not already playing
                if (!Charging.isPlaying)
                {
                    Charging.Play();
                }
                yield return null;
            }
            Charging.Stop();

            animator.SetBool("isCharging", false);
            fireBallController.transform.SetParent(null);
            fireBallController.GetComponent<BoxCollider2D>().enabled = true;
            fireBallController.GetComponent<CircleCollider2D>().enabled = true;
            if (chargeTime == attackTime3) {
                // Find the nearest enemy
                Transform nearestEnemy = FindNearestEnemy(fireBall.transform.position, "Enemy");
                if (nearestEnemy != null && mana >= manaCost)
                {
                    // Make the fireball target the nearest enemy
                    fireBall.GetComponent<FireBallController>().SetTarget(nearestEnemy);
                    mana = mana - manaCost;
                    if (MagicAtk != null)
                    {
                        MagicAtk.Play();
                    }
                }
                else
                {
                    fireBall.GetComponent<Animator>().SetTrigger("disappear");
                    yield return new WaitForSeconds(0.933f);
                    Destroy(fireBall);
                }
            }
            else
            {
                Destroy(fireBall);
            }
            // Reset states
            chargeTime = 0f;
            attacking = false;
        }
    }
    private Transform FindNearestEnemy(Vector3 origin, string enemyLayerName)
    {
        int enemyLayer = LayerMask.NameToLayer(enemyLayerName);
        Collider2D[] colliders = Physics2D.OverlapCircleAll(origin, 100f, 1 << enemyLayer); // 10f is the search radius
        Transform nearestEnemy = null;
        float closestDistance = float.MaxValue;

        foreach (var collider in colliders)
        {
            float distance = Vector2.Distance(origin, collider.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                nearestEnemy = collider.transform;
            }
        }

        return nearestEnemy;
    }

    public void AddCoin()
    {
        currentCoin++;
    }
}
