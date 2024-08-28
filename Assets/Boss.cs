using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform player;

    public void LookAtPlayer()
    {
        // Calculate the direction from the boss to the player
        Vector3 direction = player.position - transform.position;

        // Ignore the y component to only rotate on the x and z axis
        direction.y = 0;

        // If the direction is not zero, rotate the boss to look in the direction of the player
        if (direction != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }
    }
}
