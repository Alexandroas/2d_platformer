using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rb;
    private EnemyController enemyController;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        enemyController = GetComponent<EnemyController>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float horizontalVelocity = rb.velocity.x;
        if (horizontalVelocity > 0.1f)
        {
            spriteRenderer.flipX = true;
        }
        else if (horizontalVelocity < -0.1f)
        {
            spriteRenderer.flipX = false;
        }
    }
}
