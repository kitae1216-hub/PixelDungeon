using UnityEngine;

public class FloorManager : MonoBehaviour
{
    public static FloorManager Instance;

    [Header("Floor Settings")]
    [SerializeField] private int startingFloor = 1;
    [SerializeField] private int targetFloor = 3;
    [SerializeField] private bool showDebugLogs = true;

    [Header("References")]
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private EnemySpawner enemySpawner;
    [SerializeField] private ItemSpawner itemSpawner;
    [SerializeField] private PlayerController playerController;

    public int CurrentFloor { get; private set; }
    public bool IsTransitioningFloor { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            CurrentFloor = startingFloor;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (dungeonGenerator == null)
            dungeonGenerator = FindAnyObjectByType<DungeonGenerator>();

        if (enemySpawner == null)
            enemySpawner = FindAnyObjectByType<EnemySpawner>();

        if (itemSpawner == null)
            itemSpawner = FindAnyObjectByType<ItemSpawner>();

        if (playerController == null)
            playerController = FindAnyObjectByType<PlayerController>();

        DebugLog($"Current Floor: {CurrentFloor}");
    }

    public void GoToNextFloor()
    {
        if (IsTransitioningFloor)
            return;

        IsTransitioningFloor = true;
        CurrentFloor++;

        DebugLog($"Moving to Floor {CurrentFloor}");

        if (CurrentFloor >= targetFloor)
        {
            Debug.Log($"Demo Clear! Reached Floor {CurrentFloor}");
        }

        GenerateCurrentFloor();

        // 층 전환 후 입력 가능 상태로 강제 복구
        GameManager.Instance?.ForcePlayerTurnReady();

        IsTransitioningFloor = false;
    }

    public void GenerateCurrentFloor()
    {
        if (enemySpawner != null)
        {
            enemySpawner.ClearExistingEnemies();
        }

        if (itemSpawner != null)
        {
            itemSpawner.ClearExistingItems();
        }

        if (GridOccupancyManager.Instance != null)
        {
            GridOccupancyManager.Instance.ClearAll();
        }

        if (ItemPickupManager.Instance != null)
        {
            ItemPickupManager.Instance.ClearAll();
        }

        GameManager.Instance?.ClearAllEnemies();

        if (dungeonGenerator != null)
        {
            dungeonGenerator.GenerateDungeon();
        }

        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies();
        }

        if (itemSpawner != null)
        {
            itemSpawner.SpawnItems();
        }

        if (playerController != null)
        {
            GridOccupancyManager.Instance?.SetOccupant(
                playerController.GetCurrentGridPosition(),
                playerController.gameObject
            );
        }

        DebugLog($"Floor {CurrentFloor} generated.");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}