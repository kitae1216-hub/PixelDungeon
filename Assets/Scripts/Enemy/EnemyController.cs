using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private int attackPower = 2;
    [SerializeField] private float moveDuration = 0.15f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Vector2Int currentGridPosition;
    private bool isMoving = false;

    public Vector2Int CurrentGridPosition => currentGridPosition;
    public int AttackPower => attackPower;

    public void Initialize(Vector2Int spawnGridPosition)
    {
        currentGridPosition = spawnGridPosition;
        transform.position = GridManager.Instance.GridToWorld(currentGridPosition);

        GridOccupancyManager.Instance.SetOccupant(currentGridPosition, gameObject);
    }

    public IEnumerator TakeTurn(PlayerController player)
    {
        if (player == null || isMoving)
            yield break;

        Vector2Int playerPos = player.GetCurrentGridPosition();

        if (IsAdjacentTo(playerPos))
        {
            AttackPlayer(player);
            yield return new WaitForSeconds(0.1f);
            yield break;
        }

        Vector2Int direction = GetStepToward(playerPos);
        Vector2Int targetPos = currentGridPosition + direction;

        if (direction != Vector2Int.zero && CanMoveTo(targetPos))
        {
            yield return MoveRoutine(targetPos);
        }
        else
        {
            yield return new WaitForSeconds(0.05f);
        }
    }

    private bool IsAdjacentTo(Vector2Int target)
    {
        int dx = Mathf.Abs(currentGridPosition.x - target.x);
        int dy = Mathf.Abs(currentGridPosition.y - target.y);
        return dx + dy == 1;
    }

    private void AttackPlayer(PlayerController player)
    {
        Health playerHealth = player.GetComponent<Health>();
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(attackPower);
            DebugLog($"{gameObject.name} attacked Player for {attackPower}");
        }
    }

    private Vector2Int GetStepToward(Vector2Int target)
    {
        Vector2Int diff = target - currentGridPosition;

        if (Mathf.Abs(diff.x) > Mathf.Abs(diff.y))
        {
            return new Vector2Int(diff.x > 0 ? 1 : -1, 0);
        }
        else if (diff.y != 0)
        {
            return new Vector2Int(0, diff.y > 0 ? 1 : -1);
        }
        else if (diff.x != 0)
        {
            return new Vector2Int(diff.x > 0 ? 1 : -1, 0);
        }

        return Vector2Int.zero;
    }

    private bool CanMoveTo(Vector2Int targetPos)
    {
        if (!GridManager.Instance.IsWalkable(targetPos))
            return false;

        GameObject occupant = GridOccupancyManager.Instance.GetOccupant(targetPos);
        if (occupant != null)
            return false;

        return true;
    }

    private IEnumerator MoveRoutine(Vector2Int targetGridPosition)
    {
        isMoving = true;

        Vector2Int oldPos = currentGridPosition;
        Vector3 startPos = transform.position;
        Vector3 endPos = GridManager.Instance.GridToWorld(targetGridPosition);

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float eased = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPos, endPos, eased);
            yield return null;
        }

        transform.position = endPos;
        currentGridPosition = targetGridPosition;

        GridOccupancyManager.Instance.MoveOccupant(oldPos, currentGridPosition, gameObject);

        isMoving = false;
        DebugLog($"{gameObject.name} moved to {currentGridPosition}");
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}