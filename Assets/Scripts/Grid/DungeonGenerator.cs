using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)]
public class DungeonGenerator : MonoBehaviour
{
    [Header("Map Size")]
    [SerializeField] private int mapWidth = 40;
    [SerializeField] private int mapHeight = 25;

    [Header("Room Settings")]
    [SerializeField] private int roomCount = 8;
    [SerializeField] private int roomMinSize = 4;
    [SerializeField] private int roomMaxSize = 8;
    [SerializeField] private int generationAttempts = 40;

    [Header("References")]
    [SerializeField] private PlayerController playerController;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool[,] floorMap;
    private List<Room> rooms = new List<Room>();
    private List<RectInt> roomRects = new List<RectInt>();
    private Vector2Int playerSpawnPosition;
    private Vector2Int stairsPosition;

    private void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("DungeonGenerator: GridManager instance is missing.");
            return;
        }

        if (playerController == null)
        {
            playerController = FindAnyObjectByType<PlayerController>();
        }

        floorMap = new bool[mapWidth, mapHeight];
        rooms.Clear();
        roomRects.Clear();

        CreateRooms();
        ConnectRooms();

        if (rooms.Count > 0)
        {
            playerSpawnPosition = rooms[0].Center;
            stairsPosition = rooms[rooms.Count - 1].Center;
        }

        RenderMap();

        if (rooms.Count > 0 && playerController != null)
        {
            playerController.SetGridPositionImmediate(playerSpawnPosition);
            GridOccupancyManager.Instance?.ClearAll();
            GridOccupancyManager.Instance?.SetOccupant(playerSpawnPosition, playerController.gameObject);
            DebugLog($"Player spawn set to {playerSpawnPosition}");
            DebugLog($"Stairs position set to {stairsPosition}");
        }
        else
        {
            Debug.LogWarning("DungeonGenerator: No rooms generated or PlayerController not found.");
        }
    }

    public List<RectInt> GetRooms()
    {
        return new List<RectInt>(roomRects);
    }

    public Vector2Int GetPlayerSpawnPosition()
    {
        return playerSpawnPosition;
    }

    public Vector2Int GetStairsPosition()
    {
        return stairsPosition;
    }

    private void CreateRooms()
    {
        int created = 0;

        for (int i = 0; i < generationAttempts && created < roomCount; i++)
        {
            int width = Random.Range(roomMinSize, roomMaxSize + 1);
            int height = Random.Range(roomMinSize, roomMaxSize + 1);

            int x = Random.Range(1, mapWidth - width - 1);
            int y = Random.Range(1, mapHeight - height - 1);

            Room newRoom = new Room(x, y, width, height);

            bool overlaps = false;

            foreach (Room room in rooms)
            {
                if (newRoom.Overlaps(room, 1))
                {
                    overlaps = true;
                    break;
                }
            }

            if (overlaps)
                continue;

            CarveRoom(newRoom);
            rooms.Add(newRoom);
            roomRects.Add(new RectInt(newRoom.X, newRoom.Y, newRoom.Width, newRoom.Height));
            created++;
        }

        DebugLog($"Rooms created: {rooms.Count}");
    }

    private void ConnectRooms()
    {
        if (rooms.Count <= 1)
            return;

        for (int i = 1; i < rooms.Count; i++)
        {
            Vector2Int prevCenter = rooms[i - 1].Center;
            Vector2Int currentCenter = rooms[i].Center;

            if (Random.value < 0.5f)
            {
                CarveHorizontalTunnel(prevCenter.x, currentCenter.x, prevCenter.y);
                CarveVerticalTunnel(prevCenter.y, currentCenter.y, currentCenter.x);
            }
            else
            {
                CarveVerticalTunnel(prevCenter.y, currentCenter.y, prevCenter.x);
                CarveHorizontalTunnel(prevCenter.x, currentCenter.x, currentCenter.y);
            }
        }
    }

    private void CarveRoom(Room room)
    {
        for (int x = room.X; x < room.X + room.Width; x++)
        {
            for (int y = room.Y; y < room.Y + room.Height; y++)
            {
                SetFloor(x, y);
            }
        }
    }

    private void CarveHorizontalTunnel(int xStart, int xEnd, int y)
    {
        int min = Mathf.Min(xStart, xEnd);
        int max = Mathf.Max(xStart, xEnd);

        for (int x = min; x <= max; x++)
        {
            SetFloor(x, y);
        }
    }

    private void CarveVerticalTunnel(int yStart, int yEnd, int x)
    {
        int min = Mathf.Min(yStart, yEnd);
        int max = Mathf.Max(yStart, yEnd);

        for (int y = min; y <= max; y++)
        {
            SetFloor(x, y);
        }
    }

    private void SetFloor(int x, int y)
    {
        if (!IsInsideMap(x, y))
            return;

        floorMap[x, y] = true;
    }

    private bool IsInsideMap(int x, int y)
    {
        return x >= 0 && x < mapWidth && y >= 0 && y < mapHeight;
    }

    private void RenderMap()
    {
        GridManager.Instance.ClearAllTiles();

        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);

                if (floorMap[x, y])
                {
                    GridManager.Instance.SetGroundTile(pos);
                }
                else
                {
                    GridManager.Instance.SetWallTile(pos);
                }
            }
        }

        GridManager.Instance.SetStairsTile(stairsPosition);

        DebugLog("Map rendered.");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }

    [System.Serializable]
    private class Room
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Vector2Int Center => new Vector2Int(X + Width / 2, Y + Height / 2);

        public Room(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Overlaps(Room other, int padding = 0)
        {
            return X < other.X + other.Width + padding &&
                   X + Width + padding > other.X &&
                   Y < other.Y + other.Height + padding &&
                   Y + Height + padding > other.Y;
        }
    }
}