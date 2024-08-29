using System.Collections.Generic;
using UnityEngine;
using static RoomTypeAssigner;

public class GraveyardPrefabManager : MonoBehaviour
{
    public GameObject gravePrefab; // Assign this in the Inspector
    public float squareHalfSize = 5.0f; // Half the size of the square pattern around the center
    public string prefabTag = "Coffin"; // Tag for the instantiated prefabs
    public float yOffset = 0.5f; // Adjust the y position of the coffins

    public void PlacePrefabsInGraveyardRooms(List<RoomNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RoomType == RoomType.GraveyardRoom)
            {
                PlaceCoffinsInSquarePattern(room);
            }
        }
    }

    private void PlaceCoffinsInSquarePattern(RoomNode room)
    {
        // Calculate the center of the room
        Vector3 center = new Vector3(
            (room.BottomLeftAreaCorner.x + room.TopRightAreaCorner.x) / 2.0f,
            0,
            (room.BottomLeftAreaCorner.y + room.TopRightAreaCorner.y) / 2.0f
        );

        // Define positions in a square pattern around the center
        Vector3[] positions = new Vector3[]
        {
            center + new Vector3(-squareHalfSize, yOffset, -squareHalfSize), // Bottom-left corner
            center + new Vector3(squareHalfSize, yOffset, -squareHalfSize),  // Bottom-right corner
            center + new Vector3(-squareHalfSize, yOffset, squareHalfSize),  // Top-left corner
            center + new Vector3(squareHalfSize, yOffset, squareHalfSize),   // Top-right corner
            center + new Vector3(0, yOffset, -squareHalfSize),               // Bottom edge center
            center + new Vector3(0, yOffset, squareHalfSize)                 // Top edge center
        };

        // Define corresponding rotations for each position
        Quaternion[] rotations = new Quaternion[]
        {
            Quaternion.Euler(90, -45, 0), // Top-left corner
            Quaternion.Euler(90, -135, 0),  // Bottom-left corner
            Quaternion.Euler(90, 45, 0), // Bottom-right corner
            Quaternion.Euler(90, 135, 0),// Top-right corner
            Quaternion.Euler(90, 0, 0),   // Bottom edge center
            Quaternion.Euler(90, 0, 0)    // Top edge center
        };

        // Instantiate prefabs at the defined positions with corresponding rotations
        for (int i = 0; i < positions.Length; i++)
        {
            GameObject instantiatedPrefab = Instantiate(gravePrefab, positions[i], rotations[i]);

            // Set the tag of the instantiated prefab
            instantiatedPrefab.tag = prefabTag;
        }
    }
}
