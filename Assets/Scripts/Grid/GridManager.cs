using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DebugLog("GridManager initialized.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        Tilemap referenceTilemap = groundTilemap != null ? groundTilemap : wallTilemap;

        if (referenceTilemap == null)
        {
            Debug.LogError("GridManager: No reference tilemap assigned.");
            return Vector3.zero;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        Vector3 center = referenceTilemap.GetCellCenterWorld(cell);
        return new Vector3(center.x, center.y, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        Tilemap referenceTilemap = groundTilemap != null ? groundTilemap : wallTilemap;

        if (referenceTilemap == null)
        {
            Debug.LogError("GridManager: No reference tilemap assigned.");
            return Vector2Int.zero;
        }

        Vector3Int cell = referenceTilemap.WorldToCell(worldPosition);
        return new Vector2Int(cell.x, cell.y);
    }

    public bool HasGround(Vector2Int gridPosition)
    {
        if (groundTilemap == null)
        {
            Debug.LogError("GridManager: Ground Tilemap is not assigned.");
            return false;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return groundTilemap.GetTile(cell) != null;
    }

    public bool HasWall(Vector2Int gridPosition)
    {
        if (wallTilemap == null)
        {
            Debug.LogError("GridManager: Wall Tilemap is not assigned.");
            return false;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return wallTilemap.GetTile(cell) != null;
    }

    public bool IsWalkable(Vector2Int gridPosition)
    {
        bool hasGround = HasGround(gridPosition);
        bool hasWall = HasWall(gridPosition);

        if (showDebugLogs)
        {
            Debug.Log($"[IsWalkable] Pos={gridPosition}, Ground={hasGround}, Wall={hasWall}");
        }

        // 벽이 있으면 무조건 못 감
        if (hasWall)
            return false;

        // 바닥이 없으면 못 감
        if (!hasGround)
            return false;

        return true;
    }

    public bool IsValidSpawnPosition(Vector2Int gridPosition)
    {
        return IsWalkable(gridPosition);
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}