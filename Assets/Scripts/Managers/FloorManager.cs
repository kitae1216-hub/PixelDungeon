using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

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
    public int TargetFloor => targetFloor;
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

        if (GameManager.Instance != null && (GameManager.Instance.IsGameOver || GameManager.Instance.IsGameClear))
            return;

        IsTransitioningFloor = true;
        CurrentFloor++;

        MessageLog.Instance?.AddMessage($"{CurrentFloor}ÃþÀ¸·Î ÀÌµ¿");
        DebugLog($"Moving to Floor {CurrentFloor}");

        if (CurrentFloor >= targetFloor)
        {
            GameManager.Instance?.TriggerGameClear();
            IsTransitioningFloor = false;
            return;
        }

        GenerateCurrentFloor();

        GameManager.Instance?.ForcePlayerTurnReady();
        UIManager.Instance?.RefreshAll();

        IsTransitioningFloor = false;
    }

    public void GenerateCurrentFloor()
    {
        if (enemySpawner != null)
            enemySpawner.ClearExistingEnemies();

        if (itemSpawner != null)
            itemSpawner.ClearExistingItems();

        GridOccupancyManager.Instance?.ClearAll();
        ItemPickupManager.Instance?.ClearAll();
        GameManager.Instance?.ClearAllEnemies();

        dungeonGenerator?.GenerateDungeon();
        enemySpawner?.SpawnEnemies();
        itemSpawner?.SpawnItems();

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