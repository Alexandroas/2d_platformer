using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float shootingCooldown = 0.5f;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float projectileSpeed = 10f;
    private PlayerController playerController;

    [Header("Mana Settings")]
    [SerializeField] public int maxMana = 7;
    [SerializeField] private float manaRegenRate = 60.0f;

    public int currentMana;
    [Header("Skills")]
    private bool hasFireball = false;
    private float fireballCooldown = 1.0f;
    private float fireballManaCost = 1;
    private bool hasIceball = false;
    private float iceballCooldown = 1.0f;
    private float iceballManaCost = 1;
    private bool hasLightning = false;
    private float lightningCooldown = 1.0f;
    private float lightningManaCost = 1;

    
    [Header("Audio Settings (Optional)")]
    [SerializeField] private AudioClip shootSound;    
    [SerializeField] private AudioClip unvailableSkill;
    [SerializeField] private AudioClip manaEmpty;
    [SerializeField] [Range(0, 1)] private float soundVolume = 0.9f;
    private AudioSource effectsAudioSource;
    private AudioSource voiceAudioSource;

    private float nextFireTime = 0f;
    private Camera mainCamera;
    private bool canShoot = true;

    void Start()
    {
        currentMana = maxMana;
        GameObject.Find("Player").GetComponent<PlayerHealth>().UpdateManaDisplay(currentMana, maxMana);
        playerController = GetComponent<PlayerController>();
        mainCamera = Camera.main;

        // Setup effects audio source
        effectsAudioSource = gameObject.AddComponent<AudioSource>();
        effectsAudioSource.playOnAwake = false;
        effectsAudioSource.loop = false;
        effectsAudioSource.volume = soundVolume;
        effectsAudioSource.spatialBlend = 1f;

        // Setup voice audio source
        voiceAudioSource = gameObject.AddComponent<AudioSource>();
        voiceAudioSource.playOnAwake = false;
        voiceAudioSource.loop = false;
        voiceAudioSource.volume = soundVolume;
        voiceAudioSource.spatialBlend = 1f;
    }

    public int IncreaseCurrentMana(int amount)
    {
        currentMana += amount;
        currentMana = Mathf.Min(currentMana, maxMana);
        GameObject.Find("Player").GetComponent<PlayerHealth>().UpdateManaDisplay(currentMana, maxMana);
        return currentMana;
    }

    void Update()
    {
        if (currentMana < maxMana)
        {
            currentMana += (int)(manaRegenRate * Time.deltaTime);
            currentMana = Mathf.Min(currentMana, maxMana);
            GameObject.Find("Player").GetComponent<PlayerHealth>().UpdateManaDisplay(currentMana, maxMana);
        }

        if (!canShoot) return;

        if (Input.GetMouseButton(0) && Time.time >= nextFireTime)
        {
            CastFireball();
            nextFireTime = Time.time + shootingCooldown;
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
    private bool CanUseMana(float manaCost)
    {
        return currentMana >= manaCost;
    }
    private void UseMana(float amount)
    {
        currentMana -= (int)amount;
        GameObject.Find("Player").GetComponent<PlayerHealth>().UpdateManaDisplay(currentMana, maxMana);
    }
    
    public void UnlockFireball()
    {
        hasFireball = true;
        Debug.Log("Fireball unlocked!");
    }
    private void CastFireball()
    {
        if (!hasFireball)
        {
            Debug.Log("Fireball not unlocked!");
            voiceAudioSource.PlayOneShot(unvailableSkill);
            return;
        }
        if(CanUseMana(fireballManaCost))
        {
            UseMana(fireballManaCost);
            try
        {
            // Get mouse position in world coordinates
            Vector2 mousePosition = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            
            // Calculate shooting direction from firePoint to mouse position
            Vector2 direction = mousePosition - (Vector2)firePoint.position;
            direction.Normalize();

            // Create projectile
            GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
            currentMana--;
            GameObject.Find("Player").GetComponent<PlayerHealth>().UpdateManaDisplay(currentMana, maxMana);
            
            // Get the projectile component and initialize it
            Projectile projectileScript = projectile.GetComponent<Projectile>();
            if (projectileScript != null)
            {
                projectileScript.Initialize(direction);
            }

        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error in Shoot method: {e.Message}");
            canShoot = false;
        }
            Debug.Log("Fireball casted!");
        }
        else
        {
            if (manaEmpty != null)
            {
                voiceAudioSource.PlayOneShot(manaEmpty);
            }
            Debug.Log("Not enough mana!");
        }
    }
    public void UnlockIceball()
    {
        hasIceball = true;
        Debug.Log("Iceball unlocked!");
    }
    public void UnlockLightning()
    {
        hasLightning = true;
        Debug.Log("Lightning unlocked!");
    }
}