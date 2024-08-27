using System.Collections.Generic;
using UnityEngine;
using static RoomTypeAssigner;

public class TreasureRoomPrefabManager : MonoBehaviour
{
    // List of prefabs to place in the treasure room
    public List<GameObject> treasurePrefabs;
    public string prefabTag = "Treasure";

    // Number of items to place in each treasure room
    public int numberOfItemsToPlace = 10;

    // Random position offset to prevent objects from overlapping too much
    public float positionOffset = 1.0f;

    // Adjust yOffset to control the height of the prefabs
    public float defaultYOffset = 0.5f;
    public float firstYOffset = 1.0f;
    public float secondYOffset = 1.3f;

    // Dictionary to map prefabs to their specific yOffsets
    private Dictionary<GameObject, float> prefabYOffsetMap;

    private void Start()
    {
        // Initialize the prefabYOffsetMap
        prefabYOffsetMap = new Dictionary<GameObject, float>();

        // Initialize default yOffset for all prefabs
        foreach (var prefab in treasurePrefabs)
        {
            prefabYOffsetMap[prefab] = defaultYOffset;
        }

        // Set specific yOffsets for the first and second prefabs
      
        prefabYOffsetMap[treasurePrefabs[0]] = 1.0f; // First prefab
        
        
        prefabYOffsetMap[treasurePrefabs[1]] = 1.3f; // Second prefab
        
       
        prefabYOffsetMap[treasurePrefabs[1]] = 0.27f; // Third prefab
        
        
        prefabYOffsetMap[treasurePrefabs[1]] = 0.18f; // Fourth prefab
        
        
        prefabYOffsetMap[treasurePrefabs[1]] = 0.1f; // Fifth prefab
       
        prefabYOffsetMap[treasurePrefabs[1]] = 0.55f; // Sixth prefab
       
    }

    private void Test()
    {
        // Initialize the prefabYOffsetMap
        prefabYOffsetMap = new Dictionary<GameObject, float>();

        // Initialize default yOffset for all prefabs
        foreach (var prefab in treasurePrefabs)
        {
            prefabYOffsetMap[prefab] = defaultYOffset;
        }

        // Set specific yOffsets for the first and second prefabs

        prefabYOffsetMap[treasurePrefabs[0]] = 1.0f; // First prefab


        prefabYOffsetMap[treasurePrefabs[1]] = 1.15f; // Second prefab


        prefabYOffsetMap[treasurePrefabs[2]] = 0.27f; // Third prefab


        prefabYOffsetMap[treasurePrefabs[3]] = 0.18f; // Fourth prefab


        prefabYOffsetMap[treasurePrefabs[4]] = 0.1f; // Fifth prefab

        prefabYOffsetMap[treasurePrefabs[5]] = 0.07f; // Sixth prefab

    }

    public void PlaceTreasureInRooms(List<RoomNode> rooms)
    {
        foreach (var room in rooms)
        {
            if (room.RoomType == RoomType.TreasureRoom)
            {
                Test();
                PlaceTreasuresRandomly(room);
            }
        }
    }

    private void PlaceTreasuresRandomly(RoomNode room)
    {
        Debug.Log(prefabYOffsetMap);
        // Calculate the boundaries of the room
        Vector3 roomBottomLeft = new Vector3(room.BottomLeftAreaCorner.x, 0, room.BottomLeftAreaCorner.y);
        Vector3 roomTopRight = new Vector3(room.TopRightAreaCorner.x, 0, room.TopRightAreaCorner.y);

        // Place the specified number of items randomly within the room
        for (int i = 0; i < numberOfItemsToPlace; i++)
        {
            // Randomly select a prefab from the list
            GameObject prefabToPlace = treasurePrefabs[Random.Range(0, treasurePrefabs.Count)];
            

            // Determine the yOffset based on the selected prefab
            Debug.Log(prefabToPlace);
            Debug.Log(prefabYOffsetMap[prefabToPlace]);
            float yOffset = prefabYOffsetMap.ContainsKey(prefabToPlace) ? prefabYOffsetMap[prefabToPlace] : defaultYOffset;

            // Generate a random position within the room
            Vector3 randomPosition = new Vector3(
                Random.Range(roomBottomLeft.x + positionOffset, roomTopRight.x - positionOffset),
                yOffset,
                Random.Range(roomBottomLeft.z + positionOffset, roomTopRight.z - positionOffset)
            );

            // Instantiate the selected prefab at the random position
            Instantiate(prefabToPlace, randomPosition, Quaternion.identity);
            prefabToPlace.tag = "Treasure";
        }
    }
}
