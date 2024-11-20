using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private PlayerController playerController;
    private SpriteRenderer spriteRenderer;
    private readonly string IS_RUNNING = "IsRunning";
    private readonly string IS_JUMPING = "IsJumping";
    private readonly string ATTACK_TRIGGER = "Attack";
    [Header("Sound settings")]
    [SerializeField] private AudioClip attackSound;
    [SerializeField] private AudioClip walkingSound;
    private AudioSource audioSource;
    private AudioSource attackAudioSource;


    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private Vector2 attackBoxSize = new Vector2(2f, 1.5f); 
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float attackRate = 2f;
    private float nextAttackTime = 0f;
    private bool isFacingRight = true;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        playerController = GetComponent<PlayerController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.volume = 0.5f;
        audioSource.spatialBlend = 1;
        attackAudioSource = gameObject.AddComponent<AudioSource>();
        attackAudioSource.playOnAwake = false;
        attackAudioSource.loop = false;
        attackAudioSource.volume = 0.5f;
        attackAudioSource.spatialBlend = 1;

    }

    // Update is called once per frame
    void Update()
    {
        //Getting Moverment Values
        float horizontalInput = Input.GetAxis("Horizontal"); //Get the horizontal input
        float verticalVelocity = rb.velocity.y;
        bool isGrounded = playerController.GetIsGrounded();
        animator.SetBool(IS_RUNNING, horizontalInput != 0 && isGrounded);
        if (horizontalInput !=0 && isGrounded && !audioSource.isPlaying)
        {
            audioSource.clip = walkingSound;
            audioSource.Play();
        }
        else if (horizontalInput == 0 || !isGrounded)
        {
            audioSource.Stop();
        }
        animator.SetBool(IS_JUMPING, !isGrounded);
        


        if (horizontalInput > 0f) //If the player is moving horizontally
        {
            isFacingRight = true;
            spriteRenderer.flipX = false; //Set the sprite to face right
            
        }
        else if (horizontalInput < 0f) //If the player is moving horizontally
        {
            isFacingRight = false;
            spriteRenderer.flipX = true; //Set the sprite to face left
        }
        if (Input.GetKeyDown(KeyCode.Z) && Time.time >= nextAttackTime)
        {
            Attack();
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }
    void Attack()
    {
        animator.SetTrigger(ATTACK_TRIGGER);
        Vector2 attackPos = attackPoint.position;
        if (attackAudioSource != null && attackSound != null)
        {
            attackAudioSource.clip = attackSound;
            attackAudioSource.loop = false;

            attackAudioSource.PlayOneShot(attackSound);
        }
        if (!isFacingRight) //If the player is facing left
        {
            attackPos.x -= 1.5f; //Move the attack position to the left
        }
        else
        {
            attackPos.x += 1.5f; //Move the attack position to the right
        }
        Collider2D[] hitEnemies = Physics2D.OverlapBoxAll(attackPos, attackBoxSize, 0f, enemyLayer);
        foreach (Collider2D enemy in hitEnemies)
        {
            Debug.Log("We hit " + enemy.name);
            var enemyHealth = enemy.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(attackDamage, transform);
            }
            enemy.GetComponent<EnemyHealth>().TakeDamage(attackDamage);
        }
    }
    void OnDrawGizmos()
    {
        if (attackPoint == null) return;

        Gizmos.color = Color.red;
        Vector3 attackPos = attackPoint.position;
        if (!isFacingRight)
        {
            attackPos = new Vector3(attackPoint.position.x - attackBoxSize.x, attackPoint.position.y, 0);
        }
        
        // Draw attack box
        Gizmos.DrawWireCube(attackPos, attackBoxSize);
    }

    public void TriggerHurtAnimation()
    {
        animator.SetTrigger("Hurt");
    }
}
