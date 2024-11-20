using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float knockBackForce = 2.0f;
    [SerializeField] private float knockUpForce = 2.0f;
    [SerializeField] private float knockbackDuration = 0.2f;
    [SerializeField] private LayerMask groundLayer; // To check for ground
    [SerializeField] private float flashDuration = 0.1f;
    [SerializeField] private int flashCount = 3;
    [SerializeField] private Color flashColor = Color.white;
   
    private int currentHealth;
    private Rigidbody2D rb;
    private bool isKnockedBack = false;
    private float knockbackTimeLeft;
    private bool isGrounded;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private float flashTimeLeft;
    private int flashesLeft;
    private bool isFlashing = false;


    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        originalColor = spriteRenderer.color;
    }

    void FixedUpdate()
    {
        // Check if enemy is grounded
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, 0.1f, groundLayer);

        if (isKnockedBack)
        {
            knockbackTimeLeft -= Time.fixedDeltaTime;
           
            if (knockbackTimeLeft <= 0)
            {
                isKnockedBack = false;
                // Only zero out horizontal velocity
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
        else if (isGrounded)
        {
            // When grounded and not in knockback, stop horizontal movement
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
         if (isFlashing)
        {
            flashTimeLeft -= Time.fixedDeltaTime;
            
            if (flashTimeLeft <= 0)
            {
                if (spriteRenderer.color == flashColor)
                {
                    spriteRenderer.color = originalColor;
                    flashesLeft--;
                    
                    if (flashesLeft > 0)
                    {
                        flashTimeLeft = flashDuration;
                    }
                    else
                    {
                        isFlashing = false;
                    }
                }
                else
                {
                    spriteRenderer.color = flashColor;
                    flashTimeLeft = flashDuration;
                }
            }
        }

    }

    public void TakeDamage(int damage, Transform damageSource = null)
    {
        currentHealth -= damage;

        if (rb != null && damageSource != null)
        {
            float direction = transform.position.x < damageSource.position.x ? -1f : 1f;
           
            // Apply knockback
            Vector2 knockback = new Vector2(direction * -knockBackForce, knockUpForce);
            rb.velocity = knockback;
            isKnockedBack = true;
            knockbackTimeLeft = knockbackDuration;
            StartFlashing();
           
            Debug.Log($"Knockback applied: {knockback}");
        }

        Debug.Log($"Enemy took {damage} damage. Current Health: {currentHealth}");

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void StartFlashing()
    {
        isFlashing = true;
        flashTimeLeft = flashDuration;
        flashesLeft = flashCount;
        spriteRenderer.color = flashColor;
    }

    void Die()
    {
        Debug.Log("Enemy Died");
        Destroy(gameObject);
    }
}