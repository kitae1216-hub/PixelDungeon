using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(10)]
public class EnemySpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private EnemyController enemyPrefab;

    [Header("Spawn Settings")]
    [SerializeField] private int enemiesPerRoomMin = 1;
    [SerializeField] private int enemiesPerRoomMax = 2;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private readonly List<EnemyController> spawnedEnemies = new List<EnemyController>();

    private void Start()
    {
        SpawnEnemies();
    }

    public void SpawnEnemies()
    {
        if (dungeonGenerator == null)
        {
            dungeonGenerator = FindAnyObjectByType<DungeonGenerator>();
        }

        if (dungeonGenerator == null)
        {
            Debug.LogError("EnemySpawner: DungeonGenerator not found.");
            return;
        }

        if (enemyPrefab == null)
        {
            Debug.LogError("EnemySpawner: Enemy prefab is missing.");
            return;
        }

        ClearExistingEnemies();

        List<RectInt> rooms = dungeonGenerator.GetRooms();
        Vector2Int playerStart = dungeonGenerator.GetPlayerSpawnPosition();

        for (int i = 1; i < rooms.Count; i++)
        {
            RectInt room = rooms[i];
            int spawnCount = Random.Range(enemiesPerRoomMin, enemiesPerRoomMax + 1);

            for (int j = 0; j < spawnCount; j++)
            {
                Vector2Int spawnPos = GetRandomFloorPositionInRoom(room);

                if (spawnPos == playerStart)
                    continue;

                if (GridOccupancyManager.Instance.IsOccupied(spawnPos))
                    continue;

                EnemyController enemy = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity);
                enemy.name = $"Enemy_{i}_{j}";
                enemy.Initialize(spawnPos);

                spawnedEnemies.Add(enemy);
                GameManager.Instance?.RegisterEnemy(enemy);

                DebugLog($"Spawned enemy at {spawnPos}");
            }
        }
    }

    private Vector2Int GetRandomFloorPositionInRoom(RectInt room)
    {
        int x = Random.Range(room.xMin, room.xMax);
        int y = Random.Range(room.yMin, room.yMax);
        return new Vector2Int(x, y);
    }

    private void ClearExistingEnemies()
    {
        foreach (EnemyController enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy.gameObject);
            }
        }

        spawnedEnemies.Clear();
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}