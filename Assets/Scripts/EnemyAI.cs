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
    public float standStillDuration = 1.5f; // Duration to stand still after a leap or attack
    public float attackCooldown = 2.5f; // Cooldown period after attack or leap

    private Transform player;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 patrolDestination;
    public bool isChasing = false;
    private bool isLeaping = false;
    private bool grounded;
    private float nextPatrolTime;
    private float nextLeapTime;
    private float nextAttackTime;
    private Vector3 patrolCenter;
    private const float patrolRadius = 20f;

    private bool isPaused;
    private Animator animator;

    private bool isStandingStill = false; // New flag to check if standing still after leap or attack
    private float lastDamageTime = 0f; // To track the last time damage was dealt

    void Start()
    {
        // Find the child GameObject named "Mremireh O Desbiens"
        Transform childTransform = transform.Find("Mremireh O Desbiens");

        // Ensure the child GameObject exists
        if (childTransform != null)
        {
            // Get the Animator component from the child GameObject
            animator = childTransform.GetComponent<Animator>();

            // Check if the component is found to avoid errors
            if (animator == null)
            {
                Debug.LogError("Animator component not found on the child GameObject.");
            }
        }
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextPatrolTime = Time.time + patrolInterval;
        nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);
        nextAttackTime = Time.time;
        patrolCenter = transform.position;
        patrolDestination = GetRandomPatrolPosition();
    }

    void Update()
    {
        if (Time.timeScale == 0f) isPaused = true; else isPaused = false;
        if (isPaused) return;

        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        if (isStandingStill)
        {
            animator.SetTrigger("Idle");
            return; // Prevent movement or actions if standing still
        }

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
        if (isPaused || isStandingStill) return; // Prevent movement if standing still
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
        animator.SetBool("isChasing", true);

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

        if (distanceToPlayer <= 15f && Time.time >= nextLeapTime && !isLeaping)
        {
            StartCoroutine(DelayedLeap());
        }

        if (distanceToPlayer <= attackRange && Time.time >= nextAttackTime)
        {
            StartCoroutine(AttackPlayerCoroutine());
        }
    }

    private IEnumerator DelayedLeap()
    {
        isLeaping = true;
        animator.SetTrigger("JumpAttack");

        // Wait for 0.5 seconds
        yield return new WaitForSeconds(0.5f);

        // Start the leap towards player coroutine
        StartCoroutine(LeapTowardsPlayer());
    }

    private IEnumerator AttackPlayerCoroutine()
    {
        animator.SetTrigger("Attack");

        // Wait for the attack animation duration (1 second)
        yield return new WaitForSeconds(0.7f);

        // Call the AttackPlayer method to apply damage
        AttackPlayer();

        // Stand still after attacking
        yield return StandStillCoroutine();

        // Cooldown before another attack is allowed
        nextAttackTime = Time.time + attackCooldown;
    }

    IEnumerator LeapTowardsPlayer()
    {
        rb.velocity = Vector3.zero;

        Vector3 leapDirection = (player.position - transform.position).normalized;
        rb.AddForce(leapDirection * leapForce + Vector3.up * leapVerticalForce, ForceMode.Impulse);

        nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);

        yield return new WaitForSeconds(1f);

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && Time.time - lastDamageTime >= attackCooldown)
            {
                HandlePlayerCollision(hitCollider.gameObject, true, stunDuration, damage);
                lastDamageTime = Time.time; // Update last damage time
                break;
            }
        }

        isLeaping = false;

        // Stand still after the leap for the duration specified
        yield return StandStillCoroutine();

        // Cooldown before another leap or attack is allowed
        nextAttackTime = Time.time + attackCooldown;
    }

    IEnumerator StandStillCoroutine()
    {
        isStandingStill = true;
        yield return new WaitForSeconds(standStillDuration);
        isStandingStill = false;
    }

    void AttackPlayer()
    {
        Debug.Log("Attacking player");

        // Detect the player in front of the attacker or in a specific range
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player") && Time.time - lastDamageTime >= attackCooldown)
            {
                HandlePlayerCollision(hitCollider.gameObject, false, 0f, damage);
                lastDamageTime = Time.time; // Update last damage time
                break;
            }
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
        if (isLeaping)
        {
            if (flatVel.magnitude > 1.5f * moveSpeed)
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

    private void OnAttackCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject, false, 0f, damage);
        }
    }

    private void OnJumpAttackCollision(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            HandlePlayerCollision(collision.gameObject, true, stunDuration, damage);
        }
    }

    private void HandlePlayerCollision(GameObject player, bool stunPlayer, float stunDuration = 0f, int damage = 0)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            if (stunPlayer)
            {
                playerController.Stun(stunDuration);
            }
            if (damage > 0)
            {
                playerController.TakeDamage(damage);
            }
        }
    }

    // Method to stop the agent and let it only fall due to gravity
    public void StopMovement()
    {
        // Set velocity to zero to stop all movement
        rb.velocity = Vector3.zero;

        // Optionally reset angular velocity if you want to stop any rotation
        rb.angularVelocity = Vector3.zero;

        // Freeze position constraints to prevent movement but allow falling
        rb.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;

        // Optionally, you can unfreeze the constraints if you want it to fall under gravity
        // For example, you might want to unfreeze the Y-axis position constraint to let it fall
        rb.constraints = RigidbodyConstraints.FreezeRotation;

        // Optionally, set drag to zero if you have any drag affecting the rigidbody
        rb.drag = 0;
    }
}
