using System.Collections;
using UnityEngine;

[DefaultExecutionOrder(50)]
public class PlayerController : MonoBehaviour
{
    [Header("Start Position")]
    [SerializeField] private Vector2Int currentGridPosition = new Vector2Int(2, 2);

    [Header("Move Settings")]
    [SerializeField] private float moveDuration = 0.18f;
    [SerializeField] private int attackPower = 3;
    [SerializeField] private bool showDebugLogs = true;

    private bool isMoving = false;
    private bool isInitialized = false;

    private void Start()
    {
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

        if (isMoving)
            return;

        if (GameManager.Instance == null || GridManager.Instance == null)
            return;

        if (!GameManager.Instance.CanPlayerAct())
            return;

        Vector2Int input = GetMoveInput();
        if (input == Vector2Int.zero)
            return;

        TryMove(input);
    }

    private Vector2Int GetMoveInput()
    {
        // 꾹 누르고 있으면 계속 입력 유지
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
            return;
        }

        if (GridOccupancyManager.Instance != null && GridOccupancyManager.Instance.IsOccupied(targetGridPosition))
        {
            DebugLog($"Tile occupied at {targetGridPosition}");
            return;
        }

        GameManager.Instance.BeginPlayerAction();
        StartCoroutine(MoveRoutine(targetGridPosition));
    }

    private void AttackEnemy(EnemyController enemy)
    {
        Health enemyHealth = enemy.GetComponent<Health>();
        if (enemyHealth != null)
        {
            enemyHealth.TakeDamage(attackPower);
            DebugLog($"Player attacked {enemy.gameObject.name} for {attackPower}");
        }

        GameManager.Instance.BeginPlayerAction();
        GameManager.Instance.EndPlayerAction();
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

        DebugLog($"Moved to {currentGridPosition}");
        GameManager.Instance.EndPlayerAction();
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