using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;
    public Animator animator;
    public BoxCollider rightHand;
    public BoxCollider leftHand;

    public GameObject dustParticlePrefab;
    bool isEnraged = false;

    public float aoeRadius = 5f;  // Radius for the AOE attack

    public void LookAtPlayer()
    {
        // Calculate the direction from the boss to the player
        Vector3 direction = player.position - transform.position;

        // Ignore the y component to only rotate on the y-axis
        direction.y = 0;

        // If the direction is not zero, rotate the boss to look in the direction of the player
        if (direction != Vector3.zero)
        {
            // Calculate the rotation needed to look at the player
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Only apply the rotation around the y-axis
            transform.rotation = Quaternion.Euler(0, targetRotation.eulerAngles.y, 0);
        }
    }

    public void Attack()
    {
        // TODO need to wait for player to have health and a public function to decrease health with a dmg amount argument, then here depending if the boss is raging, tweak damage
        isEnraged = animator.GetBool("Enraged");

        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        // Check overlap for the right hand
        if (rightHand != null && playerCollider != null)
        {
            Collider[] rightHandOverlaps = Physics.OverlapBox(
                rightHand.bounds.center,
                rightHand.bounds.extents,
                rightHand.transform.rotation);

            foreach (var overlap in rightHandOverlaps)
            {
                if (overlap == playerCollider)
                {
                    Debug.Log("Right hand overlapped with player!");
                    // Implement attack logic for the right hand here
                    // For example, apply damage to the player
                    return;
                }
            }
        }

        if (leftHand != null && playerCollider != null)
        {
            Collider[] rightHandOverlaps = Physics.OverlapBox(
                rightHand.bounds.center,
                rightHand.bounds.extents,
                rightHand.transform.rotation);

            foreach (var overlap in rightHandOverlaps)
            {
                if (overlap == playerCollider)
                {
                    Debug.Log("Left hand overlapped with player!");
                    // Implement attack logic for the right hand here
                    // For example, apply damage to the player
                    return;
                }
            }
        }


    }

    public void Attack2()
    {
        // TODO need to wait for player to have health and a public function to decrease health with a dmg amount argument, then here depending if the boss is raging, tweak damage
        isEnraged = animator.GetBool("Enraged");

        CapsuleCollider playerCollider = player.GetComponent<CapsuleCollider>();
        // Check overlap for the right hand
        if (rightHand != null && playerCollider != null)
        {
            Collider[] rightHandOverlaps = Physics.OverlapBox(
                rightHand.bounds.center,
                rightHand.bounds.extents,
                rightHand.transform.rotation);

            foreach (var overlap in rightHandOverlaps)
            {
                if (overlap == playerCollider)
                {
                    Debug.Log("Right hand overlapped with player! in attack2");
                    // Implement attack logic for the right hand here
                    // For example, apply damage to the player
                    return;
                }
            }
        }

        if (leftHand != null && playerCollider != null)
        {
            Collider[] rightHandOverlaps = Physics.OverlapBox(
                rightHand.bounds.center,
                rightHand.bounds.extents,
                rightHand.transform.rotation);

            foreach (var overlap in rightHandOverlaps)
            {
                if (overlap == playerCollider)
                {
                    Debug.Log("Left hand overlapped with player! in attack 2");
                    // Implement attack logic for the right hand here
                    // For example, apply damage to the player
                    return;
                }
            }
        }


    }

    public void Attack3()
    {
        // TODO need to wait for player to have health and a public function to decrease health with a dmg amount argument, then here depending if the boss is raging, tweak damage
        isEnraged = animator.GetBool("Enraged");

        // Perform an AOE attack
        Collider[] hits = Physics.OverlapSphere(transform.position, aoeRadius);

        bool playerHit = false;
        foreach (var hit in hits)
        {
            if (hit.transform == player)
            {
                Debug.Log("Player hit by AOE attack!");
                playerHit = true;
                // Implement AOE attack logic, such as applying damage to the player
                break;
            }
        }

        if (dustParticlePrefab != null)
        {
            GameObject dustParticles = Instantiate(dustParticlePrefab, transform.position, Quaternion.identity);
            ParticleSystem ps = dustParticles.GetComponent<ParticleSystem>();

            if (ps != null)
            {
                ps.Play();
            }

            // Optionally, destroy the particle system after it finishes playing
            Destroy(dustParticles, ps.main.duration);
        }

        if (!playerHit)
        {
            Debug.Log("Player was not in range of the AOE attack.");
        }
    }

}
