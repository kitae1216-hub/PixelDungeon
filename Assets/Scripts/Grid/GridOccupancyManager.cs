using System.Collections.Generic;
using UnityEngine;

public class GridOccupancyManager : MonoBehaviour
{
    public static GridOccupancyManager Instance;

    private Dictionary<Vector2Int, GameObject> occupants = new Dictionary<Vector2Int, GameObject>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public bool IsOccupied(Vector2Int gridPosition)
    {
        return occupants.ContainsKey(gridPosition) && occupants[gridPosition] != null;
    }

    public GameObject GetOccupant(Vector2Int gridPosition)
    {
        if (occupants.TryGetValue(gridPosition, out GameObject obj))
        {
            return obj;
        }

        return null;
    }

    public void SetOccupant(Vector2Int gridPosition, GameObject obj)
    {
        occupants[gridPosition] = obj;
    }

    public void MoveOccupant(Vector2Int from, Vector2Int to, GameObject obj)
    {
        if (occupants.ContainsKey(from) && occupants[from] == obj)
        {
            occupants.Remove(from);
        }

        occupants[to] = obj;
    }

    public void RemoveOccupant(Vector2Int gridPosition)
    {
        if (occupants.ContainsKey(gridPosition))
        {
            occupants.Remove(gridPosition);
        }
    }

    public void RemoveOccupant(GameObject obj)
    {
        Vector2Int? foundKey = null;

        foreach (var pair in occupants)
        {
            if (pair.Value == obj)
            {
                foundKey = pair.Key;
                break;
            }
        }

        if (foundKey.HasValue)
        {
            occupants.Remove(foundKey.Value);
        }
    }

    public void ClearAll()
    {
        occupants.Clear();
    }
}