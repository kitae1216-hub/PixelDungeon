using UnityEngine;

public class PickupItem : MonoBehaviour
{
    [Header("Item Data")]
    [SerializeField] private ItemData itemData;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private Vector2Int gridPosition;

    public ItemData ItemData => itemData;
    public Vector2Int GridPosition => gridPosition;

    public void Initialize(ItemData newItemData, Vector2Int spawnGridPosition)
    {
        itemData = newItemData;
        gridPosition = spawnGridPosition;

        if (GridManager.Instance != null)
        {
            transform.position = GridManager.Instance.GridToWorld(gridPosition);
        }

        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr != null && itemData != null && itemData.icon != null)
        {
            sr.sprite = itemData.icon;
        }

        ItemPickupManager.Instance?.RegisterItem(this);

        DebugLog($"PickupItem initialized: {itemData?.itemName} at {gridPosition}");
    }

    public bool TryPickup(GameObject picker)
    {
        if (itemData == null || picker == null)
            return false;

        Inventory inventory = picker.GetComponent<Inventory>();
        if (inventory == null)
            return false;

        if (!inventory.AddItem(itemData))
            return false;

        ItemPickupManager.Instance?.UnregisterItem(this);
        Destroy(gameObject);
        return true;
    }

    private void OnDestroy()
    {
        if (ItemPickupManager.Instance != null)
        {
            ItemPickupManager.Instance.UnregisterItem(this);
        }
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}