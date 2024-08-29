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

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        moveSpeed = normalMoveSpeed;
    }

    void Update()
    {
        // Get input from the player
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        // Get the camera's forward and right vectors
        Vector3 cameraForward = playerCamera.transform.forward;
        Vector3 cameraRight = playerCamera.transform.right;

        // Make sure the camera's y component is zero
        cameraForward.y = 0;
        cameraRight.y = 0;

        // Normalize the vectors to get only the direction
        cameraForward.Normalize();
        cameraRight.Normalize();

        // Calculate movement direction relative to the camera
        moveDirection = cameraForward * verticalInput + cameraRight * horizontalInput;
        moveDirection.Normalize();

        // Check for jump input
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

        // Update player position
        if (rb.velocity.magnitude < maxSpeed)
        {
            rb.AddForce(moveDirection * moveSpeed);
        }

        // Raycast forward to detect ground in front of the player's feet
        if (Physics.Raycast(transform.position - transform.up * 0.95f, transform.forward, 1f, groundLayerMask))
        {
            // Add upward force to help the player ascend
            rb.AddForce(Vector3.up * moveSpeed * 0.4f);
        }
    }

    private void Jump()
    {
        // Raycast down to detect ground below the player
        if (Physics.Raycast(transform.position - transform.up * 1f, Vector3.down * 0.2f, 1f, groundLayerMask))
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }
}
