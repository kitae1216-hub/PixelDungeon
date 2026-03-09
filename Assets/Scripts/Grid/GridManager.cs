using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Map Size")]
    public int width = 10;
    public int height = 10;

    private HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        GenerateTestWalls();
    }

    private void GenerateTestWalls()
    {
        wallPositions.Clear();

        // 외곽 테두리 벽
        for (int x = 0; x < width; x++)
        {
            wallPositions.Add(new Vector2Int(x, 0));
            wallPositions.Add(new Vector2Int(x, height - 1));
        }

        for (int y = 0; y < height; y++)
        {
            wallPositions.Add(new Vector2Int(0, y));
            wallPositions.Add(new Vector2Int(width - 1, y));
        }

        // 내부 테스트용 벽
        wallPositions.Add(new Vector2Int(4, 4));
        wallPositions.Add(new Vector2Int(4, 5));
    }

    public bool IsInsideMap(Vector2Int position)
    {
        return position.x >= 0 && position.x < width &&
               position.y >= 0 && position.y < height;
    }

    public bool IsWalkable(Vector2Int position)
    {
        if (!IsInsideMap(position))
            return false;

        return !wallPositions.Contains(position);
    }

    public Vector3 GridToWorld(Vector2Int gridPosition)
    {
        return new Vector3(gridPosition.x, gridPosition.y, 0f);
    }

    public Vector2Int WorldToGrid(Vector3 worldPosition)
    {
        return new Vector2Int(
            Mathf.RoundToInt(worldPosition.x),
            Mathf.RoundToInt(worldPosition.y)
        );
    }

    public bool IsWall(Vector2Int position)
    {
        return wallPositions.Contains(position);
    }
}