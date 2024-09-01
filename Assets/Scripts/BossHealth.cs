using UnityEngine;

public class BossHealth : MonoBehaviour
{
    public int maxHealth = 500;   // Maximum health of the boss
    public int currentHealth;     // Current health of the boss
    public Animator animator;     // Reference to the Animator component

    private bool isBelowHalfHealth = false; // To check if the health is already below 50%

    void Start()
    {
        currentHealth = maxHealth; // Initialize current health to max health at the start
        animator = GetComponent<Animator>(); // Get the Animator component
    }

    // Method to reduce the boss's health
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Boss took damage. Current health: " + currentHealth);
        animator.SetTrigger("Hit");

        // Check if health is below 50% and if the boolean is not already set
        if (currentHealth <= maxHealth / 2 && !isBelowHalfHealth)
        {
            isBelowHalfHealth = true;
            animator.SetBool("Enraged", true);
            Debug.Log("Boss health is below 50%. Animator boolean set.");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    // Method to handle the boss's death
    private void Die()
    {
        Debug.Log("Boss is dead!");
        // Add logic for what happens when the boss dies (e.g., play animation, disable the boss, etc.)
        animator.SetBool("Dead", true);
    }
}
