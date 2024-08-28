using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth, dungeonLength;
    public int roomWidthMin, roomLengthMin;
    public int maxIterations;
    public int corridorWidth;
    public Material material;
    [Range(0.0f, 0.3f)]
    public float roomBottomCornerModifier;
    [Range(0.7f, 1.0f)]
    public float roomTopCornerMidifier;
    [Range(0, 2)]
    public int roomOffset;
    public GameObject wallVertical, wallHorizontal;
    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

    public List<RoomNode> RoomNodes { get; private set; } = new List<RoomNode>();
    public List<CorridorNode> CorridorNodes { get; private set; } = new List<CorridorNode>();
    // Start is called before the first frame update
    void Start()
    {
        
        CreateDungeon();

        
    }

    void DeleteAllEnemies()
    {
        // Find all GameObjects tagged as "Cube" and destroy them
        GameObject[] cubes = GameObject.FindGameObjectsWithTag("Enemy");
        foreach (GameObject cube in cubes)
        {
            DestroyImmediate(cube);
        }
    }

    void DeleteAllCoffins()
    {
        GameObject[] coffins = GameObject.FindGameObjectsWithTag("Coffin");
        foreach (GameObject coffin in coffins)
        {
            DestroyImmediate(coffin);
        }
    }

    void DeleteAllTreasure()
    {
        GameObject[] treasure = GameObject.FindGameObjectsWithTag("Treasure");
        foreach (GameObject trea in treasure)
        {
            DestroyImmediate(trea);
        }
    }

    void DeleteAllBooks()
    {
        GameObject[] books = GameObject.FindGameObjectsWithTag("Bookcase");
        foreach (GameObject book in books)
        {
            DestroyImmediate(book);
        }
    }

    void DeleteAltarAndCrosses()
    {
        GameObject[] altars = GameObject.FindGameObjectsWithTag("Altar");
        foreach (GameObject altar in altars)
        {
            DestroyImmediate(altar);
        }

        GameObject[] crosses = GameObject.FindGameObjectsWithTag("Cross");
        foreach (GameObject cross in crosses)
        {
            DestroyImmediate(cross);
        }
    }

    void DeleteTrapdoor()
    {
        GameObject[] trapdoors = GameObject.FindGameObjectsWithTag("Trapdoor");
        foreach (GameObject trapdoor in trapdoors)
        {
            DestroyImmediate(trapdoor);
        }
    }

    public void CreateDungeon()
    {
        DeleteTrapdoor();
        DeleteAllCoffins();
        DeleteAltarAndCrosses();
        DeleteAllTreasure();
        DeleteAllBooks();
        DeleteAllEnemies();
        DestroyAllChildren();
        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerMidifier,
            roomOffset,
            corridorWidth);

        RoomNodes.Clear();  // Clear the lists before adding new rooms and corridors
        CorridorNodes.Clear();

        foreach (var room in listOfRooms)
        {
            if (room is RoomNode)
            {
                RoomNodes.Add((RoomNode)room);
            }
            else if (room is CorridorNode)
            {
                CorridorNodes.Add((CorridorNode)room);
            }
        }
        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;
        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();
        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);
        }
        CreateWalls(wallParent);

        EnemyGenerator enemyGenerator = gameObject.AddComponent<EnemyGenerator>();
        enemyGenerator.GenerateEnemiesInRooms(RoomNodes,dungeonLength,dungeonWidth);

        RoomTypeAssigner roomTypeAssigner = new RoomTypeAssigner();
        roomTypeAssigner.AssignRoomTypes(this);
    }

    private void CreateWalls(GameObject wallParent)
    {
        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            CreateWall(wallParent, wallPosition, wallHorizontal);
        }
        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            CreateWall(wallParent, wallPosition, wallVertical);
        }
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
        }

        int[] triangles = new int[]
        {
            0,
            1,
            2,
            2,
            1,
            3
        };
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));

        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount != 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}