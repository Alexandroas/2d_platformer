using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootingCooldown = 0.5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    
    [Header("Audio Settings (Optional)")]
    [SerializeField] private AudioClip shootSound;
    private AudioSource audioSource;

    private float nextFireTime = 0f;
    private Camera mainCamera;
    private bool canShoot = true;

    void Start()
    {
        mainCamera = Camera.main;
        if (shootSound != null)
        {
            audioSource = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if (!canShoot) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + shootingCooldown;
        }
    }

    void Shoot()
    {
        if (projectilePrefab == null || firePoint == null) return;

        try
        {
            // Get mouse position in world coordinates
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // Calculate shooting direction from firePoint to mouse position
            Vector2 direction = mousePosition - (Vector2)firePoint.position;
            direction.Normalize();

            // Create projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            
            // Get the projectile component and initialize it
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(direction);
            }

            // Play sound effect if available
            if (audioSource != null && shootSound != null)
            {
                audioSource.PlayOneShot(shootSound);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in Shoot method: {e.Message}");
            canShoot = false;
        }
    }

    // Optional: For debugging
    void OnDrawGizmos()
    {
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Vector2 mousePos = Camera.main != null ? 
                Camera.main.ScreenToWorldPoint(Input.mousePosition) : Vector2.zero;
            Vector2 direction = mousePos - (Vector2)firePoint.position;
            direction.Normalize();
            
            Gizmos.DrawLine(firePoint.position, 
                (Vector2)firePoint.position + direction * 2f);
        }
    }
}