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
}
