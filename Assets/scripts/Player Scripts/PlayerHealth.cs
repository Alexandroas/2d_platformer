using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;
    [Header ("Sprite display")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Sprite[] healthSprites;
    [Header("UI Display")]
    [SerializeField] private Image heartFill; // Reference to UI fill image

    [SerializeField] private float  invincibilityTime = 1.0f;
    [SerializeField] private UnityEvent onDeath;
    [SerializeField] private UnityEvent<int> onHealthChanged;
    
    private bool isInvincible = false;
    private SpriteRenderer spriteRenderer;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        spriteRenderer = GetComponent<SpriteRenderer>();
        UpdateHealthDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void HealHealth(int healAmount)
    {
        currentHealth = Mathf.Min(currentHealth + healAmount, maxHealth);
        UpdateHealthDisplay();
    }
    private void UpdateHealthDisplay()
    {
        float healthPercentage = (float)currentHealth / maxHealth;
        Debug.Log($"Health percentage: {healthPercentage}");
        
        if (heartFill != null)
        {
            // Update the fill amount of the heart image
            heartFill.fillAmount = healthPercentage;
            Debug.Log($"Heart fill amount: {heartFill.fillAmount}");
        }
    }


    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        currentHealth -= damage;
        onHealthChanged.Invoke(currentHealth);
        if (currentHealth <= 0)
        {
            UpdateHealthDisplay();
            Die();
        }
        else
        {
            StartCoroutine(InvisibilityFrames());
            UpdateHealthDisplay();
            Debug.Log($"Player took damage. Current health {currentHealth}");
            Debug.Log($"Player is invincible: {isInvincible}");
        }
    }
    System.Collections.IEnumerator InvisibilityFrames()
    {
        isInvincible = true;
        float elapsedTime = 0;

        while (elapsedTime < invincibilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        isInvincible = false;
    }
    void Die()
    {
        onDeath.Invoke();
        GetComponent<PlayerController>().enabled = false;
        //Disable the player's collider so that it doesn't interact with other objects
        //TODO: Add Game Over screen
    }
}
