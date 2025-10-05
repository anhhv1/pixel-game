using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    [Header("Movement")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;

    public bool canDoubleJump;

    [Header("Collision info")]
    private bool isGrounded;
    private bool isAirborne;
    private float xInput;
    private float coyoteTimeCounter;
    private float jumpBufferCounter;
    private bool isWallDetected;
    [SerializeField] private float groundCheckDistance;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float coyoteTime = 0.2f;
    [SerializeField] private float jumpBufferTime = 0.2f;
    [SerializeField] private float wallCheckDistance;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        UpdateAirbornStatus();
        HandleCollisions();
        HandleInput();
        HandleAnimations(xInput);
        HandleMovement();
    }

    private void UpdateAirbornStatus()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = coyoteTime;
            if (isAirborne)
            {
                Handlelanding();
            }
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
            if (!isAirborne)
            {
                isAirborne = true;
            }
        }
    }

    private void Handlelanding()
    {
        canDoubleJump = true;
        isAirborne = false;
    }

    private void HandleInput()
    {
        xInput = Input.GetAxisRaw("Horizontal");

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferCounter = jumpBufferTime;
        }
        else
        {
            jumpBufferCounter -= Time.deltaTime;
        }

        if (jumpBufferCounter > 0)
        {
            DoubleJump();
            jumpBufferCounter = 0;
        }
    }
    private void DoubleJump()
    {
        if (isGrounded || coyoteTimeCounter > 0)
        {
            Jump();
            coyoteTimeCounter = 0;
        }
        else if (canDoubleJump)
        {
            Jump();
            canDoubleJump = false;
        }
    }
    private void Jump() => rb.velocity = new Vector2(rb.velocity.x, jumpForce);

    private void HandleCollisions()
    {
        isGrounded = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayer);
        isWallDetected = Physics2D.Raycast(transform.position, new Vector2(transform.localScale.x, 0), wallCheckDistance, groundLayer);
    }

    private void HandleAnimations(float xInput)
    {
        if (xInput != 0)
        {
            if (xInput > 0)
            {
                transform.localScale = new Vector3(1, 1, 1);
            }
            else
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
        }
        anim.SetFloat("xVelocity", rb.velocity.x);
        anim.SetFloat("yVelocity", rb.velocity.y);
        anim.SetBool("isGrounded", isGrounded);
    }

    private void HandleMovement()
    {
        rb.velocity = new Vector2(xInput * speed, rb.velocity.y);

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector2(transform.position.x, transform.position.y - groundCheckDistance));
    }
}
