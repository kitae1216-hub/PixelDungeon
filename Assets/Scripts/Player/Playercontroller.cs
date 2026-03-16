using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(50)]
public class PlayerController : MonoBehaviour
{
    [Header("Start Position")]
    [SerializeField] private Vector2Int currentGridPosition = new Vector2Int(2, 2);

    [Header("Move Settings")]
    [SerializeField] private float moveDuration = 0.18f;
    [SerializeField] private int baseAttackPower = 3;

    [Header("Input Repeat Settings")]
    [SerializeField] private float firstRepeatDelay = 0.22f;
    [SerializeField] private float repeatInterval = 0.10f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private bool isMoving = false;
    private bool isInitialized = false;

    private Vector2Int heldDirection = Vector2Int.zero;
    private Vector2Int lastExecutedDirection = Vector2Int.zero;
    private float nextRepeatTime = 0f;
    private bool hasConsumedInitialPress = false;

    private Inventory inventory;
    private EquipmentManager equipmentManager;

    private void Start()
    {
        inventory = GetComponent<Inventory>();
        equipmentManager = GetComponent<EquipmentManager>();

        if (isInitialized)
            return;

        if (GridManager.Instance == null)
        {
            Debug.LogError("PlayerController: GridManager instance is missing.");
            return;
        }

        if (!GridManager.Instance.IsValidSpawnPosition(currentGridPosition))
        {
            Debug.LogWarning($"PlayerController: Initial serialized position {currentGridPosition} is not valid yet.");
            return;
        }

        transform.position = GridManager.Instance.GridToWorld(currentGridPosition);
        GridOccupancyManager.Instance?.SetOccupant(currentGridPosition, gameObject);

        isInitialized = true;
        DebugLog($"Start Position = {currentGridPosition}");
    }

    private void Update()
    {
        if (!isInitialized)
            return;

        HandleInventoryHotkeys();
        UpdateHeldDirection();

        if (isMoving)
            return;

        if (GameManager.Instance == null || GridManager.Instance == null)
            return;

        if (!GameManager.Instance.CanPlayerAct())
            return;

        if (!ShouldProcessHeldInput())
            return;

        TryMove(heldDirection);
    }

    private void HandleInventoryHotkeys()
    {
        if (inventory == null)
            return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
            inventory.UseItemAt(0);

        if (Input.GetKeyDown(KeyCode.Alpha2))
            inventory.UseItemAt(1);

        if (Input.GetKeyDown(KeyCode.Alpha3))
            inventory.UseItemAt(2);
    }

    private void UpdateHeldDirection()
    {
        Vector2Int newDirection = GetRawHeldDirection();

        if (newDirection == Vector2Int.zero)
        {
            heldDirection = Vector2Int.zero;
            lastExecutedDirection = Vector2Int.zero;
            hasConsumedInitialPress = false;
            return;
        }

        if (newDirection != heldDirection)
        {
            heldDirection = newDirection;
            hasConsumedInitialPress = false;
            lastExecutedDirection = Vector2Int.zero;
        }
        else
        {
            heldDirection = newDirection;
        }
    }

    private Vector2Int GetRawHeldDirection()
    {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            return Vector2Int.up;

        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            return Vector2Int.down;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            return Vector2Int.left;

        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            return Vector2Int.right;

        return Vector2Int.zero;
    }

    private bool ShouldProcessHeldInput()
    {
        if (heldDirection == Vector2Int.zero)
            return false;

        if (!hasConsumedInitialPress)
        {
            hasConsumedInitialPress = true;
            lastExecutedDirection = heldDirection;
            nextRepeatTime = Time.time + firstRepeatDelay;
            return true;
        }

        if (heldDirection == lastExecutedDirection && Time.time >= nextRepeatTime)
        {
            nextRepeatTime = Time.time + repeatInterval;
            return true;
        }

        return false;
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetGridPosition = currentGridPosition + direction;
        DebugLog($"TryMove: {currentGridPosition} -> {targetGridPosition}");

        GameObject occupant = GridOccupancyManager.Instance?.GetOccupant(targetGridPosition);

        if (occupant != null && occupant != gameObject)
        {
            EnemyController enemy = occupant.GetComponent<EnemyController>();
            if (enemy != null)
            {
                AttackEnemy(enemy);
                return;
            }
        }

        bool canMove = GridManager.Instance.IsWalkable(targetGridPosition);
        if (!canMove)
        {
            DebugLog($"Blocked at {targetGridPosition}");
            nextRepeatTime = Time.time + repeatInterval;
            return;
        }

        if (GridOccupancyManager.Instance != null && GridOccupancyManager.Instance.IsOccupied(targetGridPosition))
        {
            DebugLog($"Tile occupied at {targetGridPosition}");
            nextRepeatTime = Time.time + repeatInterval;
            return;
        }

        GameManager.Instance.BeginPlayerAction();
        StartCoroutine(MoveRoutine(targetGridPosition));
    }

    private void AttackEnemy(EnemyController enemy)
    {
        int finalAttackPower = baseAttackPower;
        if (equipmentManager != null)
        {
            finalAttackPower += equipmentManager.GetAttackBonus();
        }

        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(finalAttackPower);
            DebugLog($"Player attacked {enemy.gameObject.name} for {finalAttackPower}");
        }

        GameManager.Instance.BeginPlayerAction();
        GameManager.Instance.EndPlayerAction();

        nextRepeatTime = Time.time + repeatInterval;
    }

    private IEnumerator MoveRoutine(Vector2Int targetGridPosition)
    {
        isMoving = true;

        Vector2Int oldGridPosition = currentGridPosition;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = GridManager.Instance.GridToWorld(targetGridPosition);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition, endPosition, eased);
            yield return null;
        }

        transform.position = endPosition;
        currentGridPosition = targetGridPosition;

        GridOccupancyManager.Instance?.MoveOccupant(oldGridPosition, currentGridPosition, gameObject);

        isMoving = false;

        TryAutoPickup();
        DebugLog($"Moved to {currentGridPosition}");
        GameManager.Instance.EndPlayerAction();

        nextRepeatTime = Time.time + repeatInterval;
    }

    private void TryAutoPickup()
    {
        PickupItem pickup = ItemPickupManager.Instance?.GetItemAt(currentGridPosition);
        if (pickup != null)
        {
            bool picked = pickup.TryPickup(gameObject);
            if (picked)
            {
                DebugLog($"Picked up item at {currentGridPosition}");
            }
        }
    }

    public void SetGridPositionImmediate(Vector2Int newGridPosition)
    {
        GridOccupancyManager.Instance?.RemoveOccupant(gameObject);

        currentGridPosition = newGridPosition;

        if (GridManager.Instance == null)
        {
            Debug.LogError("PlayerController: GridManager instance is missing.");
            return;
        }

        transform.position = GridManager.Instance.GridToWorld(currentGridPosition);
        GridOccupancyManager.Instance?.SetOccupant(currentGridPosition, gameObject);

        isInitialized = true;
        DebugLog($"Forced spawn position = {currentGridPosition}");
    }

    public Vector2Int GetCurrentGridPosition()
    {
        return currentGridPosition;
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}