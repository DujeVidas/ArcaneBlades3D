using System.Collections.Generic;
using UnityEngine;

public class TrapdoorPlacer : MonoBehaviour
{
    // Reference to the trapdoor prefab
    public GameObject trapdoorPrefab;

    // Y-offset to adjust the trapdoor's height (if needed)
    public float yOffset = 0.0f;

    public void PlaceTrapdoorInLastRoom(List<RoomNode> rooms)
    {
        // Ensure there are rooms in the list
        if (rooms == null || rooms.Count == 0)
        {
            Debug.LogError("No rooms available to place the trapdoor.");
            return;
        }

        // Get the last room in the list
        RoomNode lastRoom = rooms[rooms.Count - 1];

        // Calculate the center of the last room
        Vector3 roomCenter = new Vector3(
            (lastRoom.BottomLeftAreaCorner.x + lastRoom.TopRightAreaCorner.x) / 2,
            yOffset,
            (lastRoom.BottomLeftAreaCorner.y + lastRoom.TopRightAreaCorner.y) / 2
        );

        // Instantiate the trapdoor at the center of the last room
        GameObject instantiatedTrapdoor = Instantiate(trapdoorPrefab, roomCenter, Quaternion.identity);

        // Optionally, set the tag or layer of the trapdoor for identification
        instantiatedTrapdoor.tag = "Trapdoor";
    }
}
