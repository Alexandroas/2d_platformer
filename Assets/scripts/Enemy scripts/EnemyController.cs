using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2.0f;
    [SerializeField] private float patrolDistance = 3.0f;
    [SerializeField] private int damage = 1;
    [SerializeField] private LayerMask playerLayer;

    private Vector2 startPos;
    private int direction = 1;
    private Rigidbody2D rb;
    private bool isDead = false; 
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (isDead) return;
        float distanceFromStart = transform.position.x - startPos.x;

        if (Mathf.Abs(distanceFromStart) >= patrolDistance)
        {
            direction *= -1;
        }
        rb.velocity = new Vector2(direction * moveSpeed, rb.velocity.y);
    }
    void OnCollisionEnter2D(Collision2D collision) 
    {
        if (isDead) return;
        PlayerHealth playerHealth = collision.collider.GetComponent<PlayerHealth>();
        if (playerHealth != null)
        {
            ContactPoint2D contact = collision.GetContact(0);
            float angle = Vector2.Angle(contact.normal, Vector2.up); //Get the angle between the normal of the collision and the up vector
            Debug.Log ($"Collision Angle: {angle}");

            if (angle > 90f)
            {
                Debug.Log("Player is above the enemy");
                Die();
                collision.gameObject.GetComponent<Rigidbody2D>().velocity = new Vector2(0f, 7f);
            }
            else
            {
                Debug.Log("Player took damage");
                playerHealth.TakeDamage(damage);
            }
        }
    }
    void Die()
    {
        isDead = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(gameObject, 0.5f);
    }
    void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        Gizmos.color = Color.red;
        Vector3 lineStart = transform.position + Vector3.left * patrolDistance;
        Vector3 lineEnd = transform.position + Vector3.right * patrolDistance;
        Gizmos.DrawLine(lineStart, lineEnd);
    }
}
