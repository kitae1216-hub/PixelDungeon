using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] private int maxSlots = 6;
    [SerializeField] private bool showDebugLogs = true;

    private readonly List<ItemData> items = new List<ItemData>();

    public int MaxSlots => maxSlots;
    public IReadOnlyList<ItemData> Items => items;

    public bool AddItem(ItemData itemData)
    {
        if (itemData == null)
            return false;

        if (items.Count >= maxSlots)
        {
            DebugLog("Inventory full.");
            MessageLog.Instance?.AddMessage("인벤토리가 가득 찼습니다");
            return false;
        }

        items.Add(itemData);
        DebugLog($"Added item: {itemData.itemName}");
        MessageLog.Instance?.AddMessage($"{itemData.itemName} 획득");

        UIManager.Instance?.RefreshInventory();
        UIManager.Instance?.RefreshAll();
        return true;
    }

    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return false;

        DebugLog($"Removed item: {items[index].itemName}");
        items.RemoveAt(index);

        UIManager.Instance?.RefreshInventory();
        UIManager.Instance?.RefreshAll();
        return true;
    }

    public ItemData GetItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return null;

        return items[index];
    }

    public bool UseItemAt(int index)
    {
        ItemData item = GetItemAt(index);
        if (item == null)
        {
            DebugLog($"No item in slot {index}");
            return false;
        }

        Health health = GetComponent<Health>();
        EquipmentManager equipment = GetComponent<EquipmentManager>();

        switch (item.itemType)
        {
            case ItemType.Consumable:
                if (health != null && item.healAmount > 0)
                {
                    health.Heal(item.healAmount);
                    MessageLog.Instance?.AddMessage($"{item.itemName} 사용");
                    RemoveItemAt(index);
                    UIManager.Instance?.RefreshAll();
                    return true;
                }
                break;

            case ItemType.Weapon:
                if (equipment != null)
                {
                    equipment.EquipWeapon(item);
                    MessageLog.Instance?.AddMessage($"{item.itemName} 장착");
                    RemoveItemAt(index);
                    UIManager.Instance?.RefreshAll();
                    return true;
                }
                break;

            case ItemType.Armor:
                if (equipment != null)
                {
                    equipment.EquipArmor(item);
                    MessageLog.Instance?.AddMessage($"{item.itemName} 장착");
                    RemoveItemAt(index);
                    UIManager.Instance?.RefreshAll();
                    return true;
                }
                break;
        }

        return false;
    }

    public bool UseFirstConsumable()
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] != null && items[i].itemType == ItemType.Consumable)
            {
                return UseItemAt(i);
            }
        }

        DebugLog("No consumable item found.");
        MessageLog.Instance?.AddMessage("사용 가능한 포션이 없습니다");
        return false;
    }

    public bool IsFull()
    {
        return items.Count >= maxSlots;
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}