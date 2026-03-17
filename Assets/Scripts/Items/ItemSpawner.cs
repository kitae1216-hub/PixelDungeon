using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(20)]
public class ItemSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private PickupItem pickupItemPrefab;

    [Header("Spawnable Items")]
    [SerializeField] private List<ItemData> spawnableItems = new List<ItemData>();

    [Header("Spawn Settings")]
    [SerializeField] private int itemsPerRun = 4;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private readonly List<PickupItem> spawnedItems = new List<PickupItem>();

    private void Start()
    {
        SpawnItems();
    }

    public void SpawnItems()
    {
        if (dungeonGenerator == null)
        {
            dungeonGenerator = FindAnyObjectByType<DungeonGenerator>();
        }

        if (dungeonGenerator == null)
        {
            Debug.LogError("ItemSpawner: DungeonGenerator not found.");
            return;
        }

        if (pickupItemPrefab == null)
        {
            Debug.LogError("ItemSpawner: PickupItem prefab is missing.");
            return;
        }

        if (spawnableItems == null || spawnableItems.Count == 0)
        {
            Debug.LogError("ItemSpawner: No spawnable items assigned.");
            return;
        }

        ClearExistingItems();
        ItemPickupManager.Instance?.ClearAll();

        List<RectInt> rooms = dungeonGenerator.GetRooms();
        Vector2Int playerSpawn = dungeonGenerator.GetPlayerSpawnPosition();
        Vector2Int stairsPos = dungeonGenerator.GetStairsPosition();

        int spawnedCount = 0;
        int safety = 100;

        while (spawnedCount < itemsPerRun && safety > 0)
        {
            safety--;

            if (rooms.Count <= 1)
                break;

            int roomIndex = Random.Range(1, rooms.Count);
            RectInt room = rooms[roomIndex];

            Vector2Int pos = GetRandomFloorPositionInRoom(room);

            if (pos == playerSpawn)
                continue;

            if (pos == stairsPos)
                continue;

            if (GridOccupancyManager.Instance != null && GridOccupancyManager.Instance.IsOccupied(pos))
                continue;

            if (ItemPickupManager.Instance != null && ItemPickupManager.Instance.GetItemAt(pos) != null)
                continue;

            ItemData randomItem = spawnableItems[Random.Range(0, spawnableItems.Count)];

            PickupItem pickup = Instantiate(pickupItemPrefab, Vector3.zero, Quaternion.identity);
            pickup.name = $"Pickup_{randomItem.itemName}_{spawnedCount}";
            pickup.Initialize(randomItem, pos);

            spawnedItems.Add(pickup);
            spawnedCount++;

            DebugLog($"Spawned item {randomItem.itemName} at {pos}");
        }
    }

    public void ClearExistingItems()
    {
        for (int i = spawnedItems.Count - 1; i >= 0; i--)
        {
            if (spawnedItems[i] != null)
            {
                Destroy(spawnedItems[i].gameObject);
            }
        }

        spawnedItems.Clear();
    }

    private Vector2Int GetRandomFloorPositionInRoom(RectInt room)
    {
        int x = Random.Range(room.xMin, room.xMax);
        int y = Random.Range(room.yMin, room.yMax);
        return new Vector2Int(x, y);
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}