using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 12;
    public float groundDrag = 5;
    public float jumpForce = 10;
    public float jumpCooldown = 0.5f;
    public float airMultiplier = 0.4f;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;
    Vector3 moveDirection;

    Rigidbody rb;

    [Header("Player Stats")]
    public int health = 100;
    private bool isStunned = false;
    private float stunEndTime = 0f;
    public Animator animator;
    public Camera playerCamera;

    [Header("Misc")]
    public UI uiManager;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();
        SpeedControl();

        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;

        // Recover from stun if stun duration has passed
        if (isStunned && Time.time >= stunEndTime)
        {
            RecoverFromStun();
        }
    }

    private void FixedUpdate()
    {
        if (!isStunned) // Only move the player if not stunned
        {
            MovePlayer();
        }
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // when to jump
        if (Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    // Health and Combat Methods

    public void Stun(float duration)
    {
        isStunned = true;
        stunEndTime = Time.time + duration;
        rb.velocity = Vector3.zero; // Stop player movement
        Debug.Log("Player stunned for " + duration + " seconds");

        if (animator != null)
        {
            animator.SetTrigger("Stunned"); // Play stun animation if available
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log("Player took " + damage + " damage, remaining health: " + health);

        if (animator != null)
        {
            animator.SetTrigger("Damaged"); // Play damaged animation if available
        }

        if (health <= 0)
        {
            Die();
        }
    }

    private void RecoverFromStun()
    {
        isStunned = false;
        Debug.Log("Player recovered from stun.");

        if (animator != null)
        {
            animator.SetTrigger("Recover"); // Play recover animation if available
        }
    }

    private void Die()
    {
        Debug.Log("Player died.");
        // Handle player death (e.g., trigger game over, respawn, etc.)

        if (animator != null)
        {
            animator.SetTrigger("Die"); // Play death animation if available
        }

        // Optionally disable player controls or show a game over screen
        this.enabled = false; // Disables the PlayerController script
        uiManager.Death();
    }

    public void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            TakeDamage(1);
        }
    }
}
