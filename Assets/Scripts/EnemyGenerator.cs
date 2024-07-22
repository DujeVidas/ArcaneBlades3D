using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    private List<Node> enemyRooms;
    private HashSet<Vector3> occupiedPositions = new HashSet<Vector3>();
    private float minDistanceBetweenEnemies = 1.0f; // Minimum distance between enemy spawns

    public void GenerateEnemiesInRooms(List<Node> roomList, int dungeonLength, int dungeonWidth)
    {
        enemyRooms = roomList;

        if (enemyRooms.Count < 2)
        {
            Debug.LogWarning("Not enough rooms to generate enemies.");
            return;
        }

        Node firstRoom = enemyRooms[0];
        Vector2 firstRoomCenter = firstRoom.CalculateCenter();

        for (int i = 1; i < enemyRooms.Count; i++)
        {
            Node currentRoom = enemyRooms[i];
            Vector2 currentRoomCenter = currentRoom.CalculateCenter();

            // Adjust room boundaries to avoid spawning enemies inside walls
            Vector2Int adjustedBottomLeft = currentRoom.BottomLeftAreaCorner + Vector2Int.one;
            Vector2Int adjustedTopRight = currentRoom.TopRightAreaCorner - Vector2Int.one;

            currentRoom.BottomLeftAreaCorner = adjustedBottomLeft;
            currentRoom.TopRightAreaCorner = adjustedTopRight;

            // Calculate distance between firstRoom and currentRoom
            float distance = Vector2.Distance(firstRoomCenter, currentRoomCenter);

            // Calculate number of enemies based on distance
            int minEnemies = 1;
            int maxEnemies = 10;
            int numberOfEnemies = Mathf.RoundToInt(Mathf.Lerp(minEnemies, maxEnemies, distance / (dungeonWidth + dungeonLength))); // Adjust as needed

            // Spawn enemies in currentRoom
            SpawnEnemies(currentRoom, numberOfEnemies);
        }
    }

    void SpawnEnemies(Node room, int numberOfEnemies)
    {
        for (int i = 0; i < numberOfEnemies; i++)
        {
            // Generate a valid spawn position within the adjusted room
            Vector3 spawnPosition = GetValidSpawnPosition(room.BottomLeftAreaCorner, room.TopRightAreaCorner);

            // Create an enemy at the spawn position
            GameObject enemy = GameObject.CreatePrimitive(PrimitiveType.Capsule); // Replace with actual enemy prefab
            enemy.name = "Enemy";
            enemy.tag = "Enemy"; // Adjust tag as necessary
            enemy.transform.position = spawnPosition;

            // Add the spawned position to the set of occupied positions
            occupiedPositions.Add(spawnPosition);
        }
    }

    Vector3 GetValidSpawnPosition(Vector2Int bottomLeft, Vector2Int topRight)
    {
        float x, z;
        Vector3 spawnPosition;

        // Keep generating spawn positions until a valid one is found
        do
        {
            x = Random.Range(bottomLeft.x, topRight.x);
            z = Random.Range(bottomLeft.y, topRight.y);
            spawnPosition = new Vector3(x, 1, z);
        } while (!IsValidPosition(spawnPosition));

        return spawnPosition;
    }

    bool IsValidPosition(Vector3 position)
    {
        // Check if the position is not too close to any previously spawned enemy
        foreach (Vector3 occupiedPosition in occupiedPositions)
        {
            if (Vector3.Distance(position, occupiedPosition) < minDistanceBetweenEnemies)
            {
                return false;
            }
        }
        return true;
    }
}
