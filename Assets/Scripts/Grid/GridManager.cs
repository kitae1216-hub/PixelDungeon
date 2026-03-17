using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Tilemaps")]
    [SerializeField] private Tilemap groundTilemap;
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap overlayTilemap;

    [Header("Tiles")]
    [SerializeField] private TileBase groundTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase stairsTile;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = false;

    public Tilemap GroundTilemap => groundTilemap;
    public Tilemap WallTilemap => wallTilemap;
    public Tilemap OverlayTilemap => overlayTilemap;

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

    public void ClearAllTiles()
    {
        if (groundTilemap != null)
            groundTilemap.ClearAllTiles();

        if (wallTilemap != null)
            wallTilemap.ClearAllTiles();

        if (overlayTilemap != null)
            overlayTilemap.ClearAllTiles();
    }

    public void ClearOverlayTiles()
    {
        if (overlayTilemap != null)
            overlayTilemap.ClearAllTiles();
    }

    public void SetGroundTile(Vector2Int gridPosition)
    {
        if (groundTilemap == null || groundTile == null)
        {
            Debug.LogError("GridManager: Ground Tilemap or Ground Tile is missing.");
            return;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        groundTilemap.SetTile(cell, groundTile);
    }

    public void SetWallTile(Vector2Int gridPosition)
    {
        if (wallTilemap == null || wallTile == null)
        {
            Debug.LogError("GridManager: Wall Tilemap or Wall Tile is missing.");
            return;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        wallTilemap.SetTile(cell, wallTile);
    }

    public void SetStairsTile(Vector2Int gridPosition)
    {
        if (overlayTilemap == null || stairsTile == null)
        {
            Debug.LogError("GridManager: Overlay Tilemap or Stairs Tile is missing.");
            return;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        overlayTilemap.SetTile(cell, stairsTile);
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
        return groundTilemap.HasTile(cell);
    }

    public bool HasWall(Vector2Int gridPosition)
    {
        if (wallTilemap == null)
        {
            Debug.LogError("GridManager: Wall Tilemap is not assigned.");
            return false;
        }

        Vector3Int cell = new Vector3Int(gridPosition.x, gridPosition.y, 0);
        return wallTilemap.HasTile(cell);
    }

    public bool IsWalkable(Vector2Int gridPosition)
    {
        bool hasGround = HasGround(gridPosition);
        bool hasWall = HasWall(gridPosition);

        DebugLog($"[IsWalkable] Pos={gridPosition}, Ground={hasGround}, Wall={hasWall}");

        if (hasWall)
            return false;

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