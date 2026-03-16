using System.Collections.Generic;
using UnityEngine;

public class ItemPickupManager : MonoBehaviour
{
    public static ItemPickupManager Instance;

    private readonly Dictionary<Vector2Int, PickupItem> itemsByPosition = new Dictionary<Vector2Int, PickupItem>();

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

    public void RegisterItem(PickupItem item)
    {
        if (item == null)
            return;

        itemsByPosition[item.GridPosition] = item;
    }

    public void UnregisterItem(PickupItem item)
    {
        if (item == null)
            return;

        if (itemsByPosition.TryGetValue(item.GridPosition, out PickupItem existing) && existing == item)
        {
            itemsByPosition.Remove(item.GridPosition);
        }
    }

    public PickupItem GetItemAt(Vector2Int gridPosition)
    {
        if (itemsByPosition.TryGetValue(gridPosition, out PickupItem item))
        {
            return item;
        }

        return null;
    }

    public void ClearAll()
    {
        itemsByPosition.Clear();
    }
}