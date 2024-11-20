using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10.0f;
    [SerializeField] private int damage = 10;
    [SerializeField] private float lifeTime = 2.0f;
    [SerializeField] private bool rotateTowardsDirection = true;
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private float maxLineLength = 5f;

    [Header("Effects")]
    [SerializeField] private GameObject hitEffect; // Effect to play when projectile hits something
    [Header("Target Layers")]
    [SerializeField] private LayerMask targetLayers;
    [Header("VFX")]
    [SerializeField] private AudioClip flyingSound;
    [SerializeField] private AnimationClip destroyAnimation;
    [SerializeField] private bool playDestroyAnimation = true;
    [SerializeField] private AudioClip impactSound;
    [SerializeField] [Range(0f, 1f)] private float volumeScale = 0.5f;
    private Vector2 direction;
    private bool isInitialized = false;
     private bool isDestroyed = false;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = volumeScale;
        audioSource.spatialBlend = 1;

        audioSource.minDistance = 1f;
        audioSource.maxDistance = 20f;

    }
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!isInitialized || isDestroyed)
        {
            return;
        }
        rb.velocity = direction * speed;
        
    }
    public void Initialize(Vector2 dir)
    {
        direction = dir.normalized;
        isInitialized = true;
        if(rotateTowardsDirection)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
            spriteRenderer.flipX = true; // Flip sprite if moving left
        }
        else {
            spriteRenderer.flipX = false; // Reset flipX
        }
        if (animator != null)
        {
            animator.SetTrigger("Fly");
        }
        if (flyingSound != null && audioSource != null)
        {
            audioSource.clip = flyingSound;
            audioSource.Play();
            Debug.Log($"Playing flying sound:", audioSource.clip);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (isDestroyed) return;

        // Check if the other object is in one of our target layers
        if (((1 << other.gameObject.layer) & targetLayers) != 0)
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage, transform);
                Debug.Log("Hit enemy!"); // Debug log to verify hits
            }
            if (impactSound !=null && audioSource !=null)
            {
                audioSource.Stop();
                audioSource.loop = false;
                audioSource.clip = impactSound;
                audioSource.PlayOneShot(impactSound, volumeScale);
                Debug.Log($"Playing impact sound:" + impactSound.name);
            }
            DestroyProjectile();
        }
    }
    private void DestroyProjectile()
{
    if (isDestroyed) return;
    isDestroyed = true;

    // Stop movement
    rb.velocity = Vector2.zero;

    // Disable the collider to prevent multiple hits
    if (GetComponent<Collider2D>() != null)
    {
        GetComponent<Collider2D>().enabled = false;
    }

    // Hide the sprite but don't destroy yet
    if (spriteRenderer != null)
    {
        
        spriteRenderer.enabled = false;
    }

    // Let the sound play
    StartCoroutine(DestroyAfterDelay());
}
    private IEnumerator DestroyAfterDelay()
{
    // Wait for impact sound
    if (impactSound != null)
    {
        yield return new WaitForSeconds(impactSound.length + 0.1f); // Add small buffer
    }
    Destroy(gameObject);
}
    private void OnDisable()
    {
        if (audioSource != null)
        {
            audioSource.Stop();
        }
        StopAllCoroutines();
    }

}
