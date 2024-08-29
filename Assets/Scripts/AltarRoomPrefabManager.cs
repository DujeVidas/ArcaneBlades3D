using System.Collections.Generic;
using UnityEngine;
using static RoomTypeAssigner;

public class AltarRoomPrefabManager : MonoBehaviour
{
    // Altar prefab to place in the altar room
    public GameObject altarPrefab;

    // Cross prefab to place around the altar
    public GameObject crossPrefab;

    // Number of crosses to place around the altar
    public int numberOfCrosses = 4;

    // Distance from the center of the room to place the crosses
    public float crossRadius = 3.0f;

    // Adjust yOffset to control the height of the altar and crosses
    public float defaultYOffset = 0.0f;

    public void PlaceAltarAndCrossesInRooms(List<RoomNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RoomType == RoomType.AltarRoom)
            {
                PlaceAltarAndCrosses(room);
            }
        }
    }

    private void PlaceAltarAndCrosses(RoomNode room)
    {
        // Calculate the center of the room
        Vector3 roomCenter = new Vector3(
            (room.BottomLeftAreaCorner.x + room.TopRightAreaCorner.x) / 2,
            defaultYOffset,
            (room.BottomLeftAreaCorner.y + room.TopRightAreaCorner.y) / 2
        );

        // Instantiate the altar at the center of the room
        GameObject instantiatedAltar = Instantiate(altarPrefab, roomCenter, Quaternion.identity);

        // Set the tag of the instantiated altar
        instantiatedAltar.tag = "Altar";

        // Place the crosses in a circle around the altar
        for (int i = 0; i < numberOfCrosses; i++)
        {
            // Calculate the angle for this cross
            float angle = i * Mathf.PI * 2 / numberOfCrosses;

            // Calculate the position for the cross
            Vector3 crossPosition = new Vector3(
                roomCenter.x + Mathf.Cos(angle) * crossRadius,
                1.35f,
                roomCenter.z + Mathf.Sin(angle) * crossRadius
            );

            // Instantiate the cross at the calculated position
            GameObject instantiatedCross = Instantiate(crossPrefab, crossPosition, Quaternion.identity);

            // Rotate the cross to face the center of the room (towards the altar)
            instantiatedCross.transform.LookAt(roomCenter);

            // Rotate the cross 90 degrees on the X-axis
            instantiatedCross.transform.Rotate(-90f, 0f, 0f);

            // Set the tag of the instantiated cross
            instantiatedCross.tag = "Cross";
        }
    }
}
