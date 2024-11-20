using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float MoveSpeed = 5.0f;
    [SerializeField] private float jumpForce= 7.0f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private Rigidbody2D rb;
    private float horizontalInput;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public (bool isMoving, float currentSpeed) GetMovementInfo()
    {
        bool isMoving = Mathf.Abs(horizontalInput) > 0.1f;
        float currentSpeed = Mathf.Abs(horizontalInput * MoveSpeed);
        return (isMoving, currentSpeed);
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        }
    }
    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * MoveSpeed, rb.velocity.y);
    }
    public bool GetIsGrounded()
    {
        return isGrounded;
    }
}
