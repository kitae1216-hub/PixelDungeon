using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Vector2Int currentGridPosition = new Vector2Int(2, 2);

    private void Start()
    {
        if (GridManager.Instance == null)
        {
            Debug.LogError("GridManager instance is missing.");
            return;
        }

        transform.position = GridManager.Instance.GridToWorld(currentGridPosition);
    }

    private void Update()
    {
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
        GameManager.Instance.BeginPlayerAction();

        Vector2Int targetPosition = currentGridPosition + direction;

        if (GridManager.Instance.IsWalkable(targetPosition))
        {
            currentGridPosition = targetPosition;
            transform.position = GridManager.Instance.GridToWorld(currentGridPosition);
        }
        else
        {
            Debug.Log("Blocked by wall");
        }

        GameManager.Instance.EndPlayerAction();
    }
}