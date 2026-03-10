using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Start Position")]
    [SerializeField] private Vector2Int currentGridPosition = new Vector2Int(2, 2);

    [Header("Move Settings")]
    [SerializeField] private float moveDuration = 0.18f;
    [SerializeField] private bool showDebugLogs = true;

    private bool isMoving = false;

    private void Start()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("PlayerController: GridManager instance is missing.");
            return;
        }

        if (!GridManager.Instance.IsValidSpawnPosition(currentGridPosition))
        {
            Debug.LogError($"PlayerController: Invalid start position {currentGridPosition}");
            return;
        }

        transform.position = GridManager.Instance.GridToWorld(currentGridPosition);
        DebugLog($"Start Position = {currentGridPosition}");
    }

    private void Update()
    {
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
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            return Vector2Int.up;

        if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
            return Vector2Int.down;

        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            return Vector2Int.left;

        if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            return Vector2Int.right;

        return Vector2Int.zero;
    }

    private void TryMove(Vector2Int direction)
    {
        Vector2Int targetGridPosition = currentGridPosition + direction;

        DebugLog($"TryMove: {currentGridPosition} -> {targetGridPosition}");

        // 이동 전에 먼저 검사
        bool canMove = GridManager.Instance.IsWalkable(targetGridPosition);

        if (!canMove)
        {
            DebugLog($"Blocked at {targetGridPosition}");
            return;
        }

        GameManager.Instance.BeginPlayerAction();
        StartCoroutine(MoveRoutine(targetGridPosition));
    }

    private IEnumerator MoveRoutine(Vector2Int targetGridPosition)
    {
        isMoving = true;

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
        isMoving = false;

        DebugLog($"Moved to {currentGridPosition}");
        GameManager.Instance.EndPlayerAction();
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}