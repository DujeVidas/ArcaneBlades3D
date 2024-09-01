using System.Collections;
using TMPro;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float normalMoveSpeed = 50f;
    public float sprintSpeed = 75f;
    public float moveSpeed = 20f;
    public float maxSpeed = 30f;
    public float rotateSpeed = 8f;
    public LayerMask groundLayerMask;
    public float jumpForce = 10f;
    public Animator animator;
    public Camera playerCamera;

    private Vector3 moveDirection;
    private Rigidbody rb;
    private Vector3 lookDirection;

    public int health = 100;
    private bool isStunned = false;
    private float stunEndTime = 0f;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = normalMoveSpeed;
    }

    void Update()
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
        moveDirection.Normalize();

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }
    }

    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = sprintSpeed;
        }
        else
        {
            moveSpeed = normalMoveSpeed;
        }

        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * moveSpeed);
        }

        if (Physics.Raycast(transform.position - transform.up * 0.95f, transform.forward, 1f, groundLayerMask))
        {
            rb.AddForce(Vector3.up * moveSpeed * 0.4f);
        }
    }

    private void Jump()
    {
        if (Physics.Raycast(transform.position, Vector3.down, 1.01f, groundLayerMask))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            Debug.Log("ground not detected");
        }
    }



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
    }

}
