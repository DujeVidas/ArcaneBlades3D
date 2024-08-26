using System.Collections.Generic;
using UnityEngine;
using static RoomTypeAssigner;

public class GraveyardPrefabManager : MonoBehaviour
{
    public GameObject gravePrefab; // Assign this in the Inspector
    public float prefabSpacing = 2.0f; // Adjust spacing based on your prefab size

    public void PlacePrefabsInGraveyardRooms(List<RoomNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RoomType == RoomType.GraveyardRoom)
            {
                PlacePrefabsAlongWalls(room);
            }
        }
    }

    private void PlacePrefabsAlongWalls(RoomNode room)
    {
        // Define the corners of the room
        Vector3 bottomLeft = new Vector3(room.BottomLeftAreaCorner.x, 0, room.BottomLeftAreaCorner.y);
        Vector3 bottomRight = new Vector3(room.TopRightAreaCorner.x, 0, room.BottomLeftAreaCorner.y);
        Vector3 topLeft = new Vector3(room.BottomLeftAreaCorner.x, 0, room.TopRightAreaCorner.y);
        Vector3 topRight = new Vector3(room.TopRightAreaCorner.x, 0, room.TopRightAreaCorner.y);

        // Place prefabs along each wall
        PlacePrefabsOnLine(bottomLeft, bottomRight); // Bottom wall
        PlacePrefabsOnLine(bottomRight, topRight); // Right wall
        PlacePrefabsOnLine(topRight, topLeft); // Top wall
        PlacePrefabsOnLine(topLeft, bottomLeft); // Left wall
    }

    private void PlacePrefabsOnLine(Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        int prefabCount = Mathf.FloorToInt(distance / prefabSpacing);

        for (int i = 0; i <= prefabCount; i++)
        {
            Vector3 position = Vector3.Lerp(start, end, i / (float)prefabCount);
            Instantiate(gravePrefab, position, Quaternion.identity);
        }
    }
}
