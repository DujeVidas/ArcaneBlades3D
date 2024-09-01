using UnityEngine;
using System.Collections;

public class EnemyAI : MonoBehaviour
{
    [Header("Movement Settings")]
    private float moveSpeed = 5f; // General movement speed
    public float groundDrag = 5f;
    public float airMultiplier = 0.4f;
    public float patrolSpeed = 4f;
    public float chaseSpeed = 7f;
    public float patrolInterval = 5f;
    public float leapCooldownMin = 8f;
    public float leapCooldownMax = 10f;
    public float leapVerticalForce = 8f;
    public float stunDuration = 1f;
    public int damage = 10;
    public LayerMask playerLayer;
    public LayerMask whatIsGround;
    public float playerHeight = 2f;

    [Header("Combat Settings")]
    public float chaseRange = 50f;
    public float attackRange = 2f;
    public float leapForce = 35f;

    private Transform player;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 patrolDestination;
    private bool isChasing = false;
    private bool isLeaping = false;
    private bool readyToJump = true;
    private bool grounded;
    private float nextPatrolTime;
    private float nextLeapTime;
    private Vector3 patrolCenter;
    private const float patrolRadius = 20f;

    private bool isPaused;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextPatrolTime = Time.time + patrolInterval;
        nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);
        patrolCenter = transform.position;
        patrolDestination = GetRandomPatrolPosition();
    }

    void Update()
    {
        if(Time.timeScale == 0f) isPaused = true; else isPaused = false;
        if(isPaused) return;
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (!isChasing)
        {
            moveSpeed = patrolSpeed;
            Patrol();
            CheckForPlayer();
        }
        else
        {
            moveSpeed = chaseSpeed;
            ChasePlayer();
        }

        RotateTowardsMovementDirection();
        SpeedControl();
        
        // Handle drag
        rb.drag = grounded ? groundDrag : 0;
    }

    void FixedUpdate()
    {
        if(isPaused) return;
        MoveEnemy();
    }

    void Patrol()
    {
        moveDirection = (patrolDestination - transform.position).normalized;

        // Move towards the patrol destination
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * patrolSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * patrolSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        if (Vector3.Distance(transform.position, patrolDestination) < 1f)
        {
            rb.velocity = Vector3.zero;
        }

        if (Time.time >= nextPatrolTime)
        {
            patrolDestination = GetRandomPatrolPosition();
            nextPatrolTime = Time.time + patrolInterval;
        }
    }

    void CheckForPlayer()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        if (distanceToPlayer <= chaseRange)
        {
            isChasing = true;
        }
    }

    void ChasePlayer()
    {
        moveDirection = (player.position - transform.position).normalized;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * chaseSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * chaseSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= 15f && Time.time >= nextLeapTime && !isLeaping && grounded)
        {
            StartCoroutine(LeapTowardsPlayer());
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

    IEnumerator LeapTowardsPlayer()
    {
        isLeaping = true;
        rb.velocity = Vector3.zero;

        Vector3 leapDirection = (player.position - transform.position).normalized;
        rb.AddForce(leapDirection * leapForce + Vector3.up * leapVerticalForce, ForceMode.Impulse);

        nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);

        yield return new WaitForSeconds(1f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                StunAndDamagePlayer(hitCollider.gameObject);
                break;
            }
        }
        isLeaping = false;
    }

    void AttackPlayer()
    {
        Debug.Log("Attacking player");
    }

    void StunAndDamagePlayer(GameObject player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Stun(stunDuration);
            playerController.TakeDamage(damage);
        }
    }

    void MoveEnemy()
    {
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        if(isLeaping) {
            if (flatVel.magnitude > 1.5 * moveSpeed)
                {
                    Vector3 limitedVel = flatVel.normalized * moveSpeed;
                    rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
                }
                return;
        }
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    Vector3 GetRandomPatrolPosition()
    {
        Vector3 randomPosition = patrolCenter;
        bool foundValidPosition = false;
        int maxAttempts = 10;

        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomCirclePoint = Random.insideUnitCircle * patrolRadius;
            Vector3 potentialPosition = new Vector3(randomCirclePoint.x, transform.position.y, randomCirclePoint.y) + patrolCenter;

            RaycastHit hit;
            if (Physics.Raycast(potentialPosition + Vector3.up * 50f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                if (hit.collider.CompareTag("Floor"))
                {
                    randomPosition = hit.point;
                    foundValidPosition = true;
                    break;
                }
            }
        }

        return randomPosition;
    }

void RotateTowardsMovementDirection()
{
    if (moveDirection != Vector3.zero)
    {
        // Create a target rotation based on the movement direction
        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        // Extract the Y component of the target rotation
        float targetYRotation = targetRotation.eulerAngles.y;

        // Create a new rotation that only includes the Y component
        Quaternion yRotationOnly = Quaternion.Euler(0, targetYRotation, 0);

        // Smoothly rotate the object towards the Y-axis rotation
        transform.rotation = Quaternion.Slerp(transform.rotation, yRotationOnly, Time.deltaTime * 2f);
    }
}

}
