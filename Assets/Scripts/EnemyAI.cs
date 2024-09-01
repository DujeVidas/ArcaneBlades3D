using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float patrolSpeed = 1.5f; // Move speed during patrol
    public float chaseSpeed = 7f; // Speed when chasing
    public float maxSpeed = 7f;
    public float patrolInterval = 5f;
    public float chaseRange = 50f;
    public float attackRange = 2f;
    public float leapCooldownMin = 8f;
    public float leapCooldownMax = 10f;
    public float leapForce = 15f;
    public float leapVerticalForce = 1f; // Added vertical force for leap
    public float stunDuration = 1f;
    public int damage = 10;
    public LayerMask playerLayer;

    private Transform player;
    private Rigidbody rb;
    private Vector3 moveDirection;
    private Vector3 patrolDestination; // Current patrol destination
    private bool isChasing = false;
    private bool isLeaping = false;
    private float nextPatrolTime;
    private float nextLeapTime;
    private Vector3 patrolCenter; // Central point of patrol circle
    private const float patrolRadius = 20f; // Fixed radius of patrol circle

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        nextPatrolTime = Time.time + patrolInterval;
        nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);

        // Set the patrol center to the current position
        patrolCenter = transform.position;

        // Initialize the first patrol destination
        patrolDestination = GetRandomPatrolPosition();
    }

    void Update()
    {
        if (!isChasing)
        {
            Patrol();
            CheckForPlayer();
        }
        else
        {
            ChasePlayer();
        }
    }

    void FixedUpdate()
    {
        if (isChasing)
        {
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * chaseSpeed);
            }
        }
        else
        {
            if (rb.velocity.magnitude < maxSpeed)
            {
                rb.AddForce(moveDirection * patrolSpeed);
            }
        }
    }

    void Patrol()
    {
        // Move towards the patrol destination
        moveDirection = (patrolDestination - transform.position).normalized;
        rb.velocity = moveDirection * patrolSpeed;

        // Check if the enemy has reached the patrol destination
        if (Vector3.Distance(transform.position, patrolDestination) < 1f)
        {
            // Stop moving and wait for the next patrol interval
            rb.velocity = Vector3.zero;
        }

        // Update patrol destination based on time
        if (Time.time >= nextPatrolTime)
        {
            // Set a new patrol destination
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
        rb.AddForce(moveDirection * chaseSpeed);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= 15f && Time.time >= nextLeapTime && !isLeaping)
        {
            StartCoroutine(LeapTowardsPlayer());
        }

        if (distanceToPlayer <= attackRange)
        {
            AttackPlayer();
        }
    }

System.Collections.IEnumerator LeapTowardsPlayer()
{
    rb.velocity = Vector3.zero; // Reset velocity to zero

    // Leap logic
    Vector3 leapDirection = (player.position - transform.position).normalized;
    rb.AddForce(leapDirection * leapForce + Vector3.up * leapVerticalForce, ForceMode.Impulse);

    float leapDuration = 1f; // Duration for checking if player is hit
    float endTime = Time.time + leapDuration;

    // Continuously check if we hit the player for the next 1 second
    while (Time.time < endTime)
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange, playerLayer);
        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.CompareTag("Player"))
            {
                StunAndDamagePlayer(hitCollider.gameObject);
                // Optionally, you can break out of the loop if you want to stop checking after hitting the player
                break;
            }
        }

        yield return new WaitForSeconds(0.1f); // Check every 0.1 seconds
    }

    // Re-enable physics and set the next leap time
    nextLeapTime = Time.time + Random.Range(leapCooldownMin, leapCooldownMax);
}

    void AttackPlayer()
    {
        // Attack logic (e.g., reduce health)
        Debug.Log("Attacking player");
    }

    void StunAndDamagePlayer(GameObject player)
    {
        // Apply stun and damage
        PlayerController playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.Stun(stunDuration);
            playerController.TakeDamage(damage);
        }
    }

    Vector3 GetRandomPatrolPosition()
    {
        Vector3 randomPosition = patrolCenter; // Default to patrol center if no valid position is found
        bool foundValidPosition = false;
        int maxAttempts = 10; // Maximum number of attempts to find a valid position

        for (int i = 0; i < maxAttempts; i++)
        {
            // Get a random point within a circle centered on the patrol center
            Vector2 randomCirclePoint = Random.insideUnitCircle * patrolRadius;
            Vector3 potentialPosition = new Vector3(randomCirclePoint.x, transform.position.y, randomCirclePoint.y) + patrolCenter;

            // Perform a raycast downwards from the potential position to check if it hits the floor
            RaycastHit hit;
            // Cast the ray down from a higher point to ensure it intersects with the floor
            if (Physics.Raycast(potentialPosition + Vector3.up * 50f, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Default")))
            {
                // If the raycast hit an object with the tag "Floor"
                if (hit.collider.CompareTag("Floor"))
                {
                    randomPosition = hit.point; // Use the exact hit point on the floor
                    foundValidPosition = true;
                    break;
                }
            }
        }

        return randomPosition;
    }
}
