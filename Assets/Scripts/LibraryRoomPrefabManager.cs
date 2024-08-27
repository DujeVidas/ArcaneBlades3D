using System.Collections.Generic;
using UnityEngine;
using static RoomTypeAssigner;

public class LibraryRoomPrefabManager : MonoBehaviour
{
    // List of bookcase prefabs to place in the library room
    public List<GameObject> bookcasePrefabs;
    public string prefabTag = "Bookcase";

    // Number of bookcases to place in each library room
    public int numberOfBookcasesToPlace = 10;

    // Random position offset to prevent objects from overlapping too much
    public float positionOffset = 1.0f;

    // Adjust yOffset to control the height of the bookcases
    public float defaultYOffset = 0.0f;

    // Maximum rotation angle on the Y-axis
    public float maxYRotation = 360.0f;


    private Dictionary<GameObject, float> prefabYOffsetMap;

    private void Test()
    {
        // Initialize the prefabYOffsetMap
        prefabYOffsetMap = new Dictionary<GameObject, float>();

        // Initialize default yOffset for all prefabs
        foreach (var prefab in bookcasePrefabs)
        {
            prefabYOffsetMap[prefab] = defaultYOffset;
        }

        // Set specific yOffsets for the first and second prefabs

        prefabYOffsetMap[bookcasePrefabs[2]] = 0.025f; // First prefab


        prefabYOffsetMap[bookcasePrefabs[3]] = 0.025f; // Second prefab


        prefabYOffsetMap[bookcasePrefabs[4]] = 0.025f; // Third prefab


        prefabYOffsetMap[bookcasePrefabs[5]] = 0.025f; // Fourth prefab


     

    }

    public void PlaceBookcasesInRooms(List<RoomNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RoomType == RoomType.LibraryRoom)
            {
                Test();
                PlaceBookcasesRandomly(room);
            }
        }
    }

    private void PlaceBookcasesRandomly(RoomNode room)
    {
        // Calculate the boundaries of the room
        Vector3 roomBottomLeft = new Vector3(room.BottomLeftAreaCorner.x+1, 0, room.BottomLeftAreaCorner.y+1);
        Vector3 roomTopRight = new Vector3(room.TopRightAreaCorner.x-1, 0, room.TopRightAreaCorner.y-1);

        // Place the specified number of bookcases randomly within the room
        for (int i = 0; i < numberOfBookcasesToPlace; i++)
        {
            // Randomly select a bookcase prefab from the list
            GameObject bookcaseToPlace = bookcasePrefabs[Random.Range(0, bookcasePrefabs.Count)];
            float yOffset = prefabYOffsetMap.ContainsKey(bookcaseToPlace) ? prefabYOffsetMap[bookcaseToPlace] : defaultYOffset;
            // Generate a random position within the room
            Vector3 randomPosition = new Vector3(
                Random.Range(roomBottomLeft.x + positionOffset, roomTopRight.x - positionOffset),
                yOffset,
                Random.Range(roomBottomLeft.z + positionOffset, roomTopRight.z - positionOffset)
            );

            // Generate a random rotation around the Y-axis
            float randomYRotation = Random.Range(0f, maxYRotation);
            Quaternion randomRotation = Quaternion.Euler(0, randomYRotation, 0);

            // Instantiate the selected bookcase at the random position with random rotation
            GameObject instantiatedBookcase = Instantiate(bookcaseToPlace, randomPosition, randomRotation);

            // Set the tag of the instantiated bookcase
            instantiatedBookcase.tag = prefabTag;
        }
    }
}
