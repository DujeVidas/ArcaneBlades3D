using System.Collections.Generic;
using UnityEngine;

public class RoomTypeAssigner : MonoBehaviour
{
    public enum RoomType
    {
        TreasureRoom,
        MonsterRoom,
        TrapRoom,
        RestRoom,
        LibraryRoom,
        AltarRoom,
        GraveyardRoom
    }

    public void AssignRoomTypes(DungeonCreator dungeonCreator)
    {
        Debug.Log("Here");
        // Get the list of rooms from the DungeonCreator
        List<RoomNode> rooms = dungeonCreator.RoomNodes;

        // Ensure there are enough rooms
        if (rooms.Count < 8)
        {
            Debug.LogError("Not enough rooms to fulfill all requirements.");
            return;
        }

        // Assign the first and last rooms as Rest Rooms
        rooms[0].RoomType = RoomType.RestRoom;
        rooms[rooms.Count - 1].RoomType = RoomType.RestRoom;

        // Create a list of the required room types
        List<RoomType> requiredRoomTypes = new List<RoomType>
        {
            RoomType.TreasureRoom,
            RoomType.TrapRoom,
            RoomType.TrapRoom,
            RoomType.LibraryRoom,
            RoomType.AltarRoom,
            RoomType.GraveyardRoom
        };

        // Assign the required room types to the rooms in order
        int roomIndex = 1; // Start from the second room

        foreach (var roomType in requiredRoomTypes)
        {
            if (roomIndex < rooms.Count - 1) // Ensure we don't go past the last room
            {
                rooms[roomIndex].RoomType = roomType;
                roomIndex++;
            }
        }

        // Assign the remaining rooms as Monster Rooms
        while (roomIndex < rooms.Count - 1) // The last room is already assigned as Rest Room
        {
            rooms[roomIndex].RoomType = RoomType.MonsterRoom;
            roomIndex++;
        }

        // Debug log to verify the assignment
        foreach (var room in rooms)
        {
            Debug.Log($"Room at ({room.BottomLeftAreaCorner}, {room.TopRightAreaCorner}) is of type {room.RoomType}");
        }
    }
}
