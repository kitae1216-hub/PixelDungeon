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
            return false;
        }

        items.Add(itemData);
        DebugLog($"Added item: {itemData.itemName}");
        PrintInventory();
        return true;
    }

    public bool RemoveItemAt(int index)
    {
        if (index < 0 || index >= items.Count)
            return false;

        DebugLog($"Removed item: {items[index].itemName}");
        items.RemoveAt(index);
        PrintInventory();
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
                    DebugLog($"Used consumable: {item.itemName}, healed {item.healAmount}");
                    RemoveItemAt(index);
                    return true;
                }
                break;

            case ItemType.Weapon:
                if (equipment != null)
                {
                    equipment.EquipWeapon(item);
                    DebugLog($"Equipped weapon: {item.itemName}");
                    RemoveItemAt(index);
                    return true;
                }
                break;

            case ItemType.Armor:
                if (equipment != null)
                {
                    equipment.EquipArmor(item);
                    DebugLog($"Equipped armor: {item.itemName}");
                    RemoveItemAt(index);
                    return true;
                }
                break;
        }

        return false;
    }

    public bool IsFull()
    {
        return items.Count >= maxSlots;
    }

    public void PrintInventory()
    {
        if (!showDebugLogs)
            return;

        string result = "Inventory: ";
        if (items.Count == 0)
        {
            result += "(empty)";
        }
        else
        {
            for (int i = 0; i < items.Count; i++)
            {
                result += $"[{i}] {items[i].itemName} ";
            }
        }

        Debug.Log(result);
    }

    private void DebugLog(string message)
    {
        if (showDebugLogs)
        {
            Debug.Log(message);
        }
    }
}