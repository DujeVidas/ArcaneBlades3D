using System.Collections;
using UnityEngine;

public class ShootableObject : MonoBehaviour
{
    public float health = 5;
    public bool destroyOnDeath = true;

    private Animator animator;
    private EnemyAI enemyAI; // Reference to the EnemyAI script

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

        // Get the EnemyAI component on the same GameObject
        enemyAI = GetComponent<EnemyAI>();
        if (enemyAI == null)
        {
            Debug.LogError("EnemyAI component not found on this GameObject.");
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            if (destroyOnDeath)
            {
                // Trigger the "Dead" animation
                animator.SetTrigger("Dead");

                // Stop the enemy's movement
                if (enemyAI != null)
                {
                    enemyAI.StopMovement();
                }

                // Start the coroutine to destroy the GameObject after 3 seconds
                StartCoroutine(DestroyAfterDelay(0f));
            }
            else
            {
                gameObject.SetActive(false); // maybe add death animation
            }
        }
    }

    private IEnumerator DestroyAfterDelay(float delay)
    {
        // Wait for the specified delay
        yield return new WaitForSeconds(delay);

        // Destroy the GameObject
        Destroy(gameObject);
    }

    public void TakeShot(float bulletDamage)
    {
        health -= bulletDamage;

        // Set isChasing to true when the object takes a shot
        if (enemyAI != null)
        {
            enemyAI.isChasing = true; // Change the chasing state to true
        }
    }
}
