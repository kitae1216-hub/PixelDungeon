using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    [Header("Equipped Items")]
    [SerializeField] private ItemData equippedWeapon;
    [SerializeField] private ItemData equippedArmor;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    public ItemData EquippedWeapon => equippedWeapon;
    public ItemData EquippedArmor => equippedArmor;

    public void EquipWeapon(ItemData itemData)
    {
        if (itemData == null || itemData.itemType != ItemType.Weapon)
            return;

        equippedWeapon = itemData;

        DebugLog($"Weapon equipped: {itemData.itemName}");
        MessageLog.Instance?.AddMessage($"{itemData.itemName} ÀåÂø");

        ForceRefreshUI();
    }

    public void EquipArmor(ItemData itemData)
    {
        if (itemData == null || itemData.itemType != ItemType.Armor)
            return;

        equippedArmor = itemData;

        DebugLog($"Armor equipped: {itemData.itemName}");
        MessageLog.Instance?.AddMessage($"{itemData.itemName} ÀåÂø");

        ForceRefreshUI();
    }

    public int GetAttackBonus()
    {
        return equippedWeapon != null ? equippedWeapon.attackBonus : 0;
    }

    public int GetDefenseBonus()
    {
        return equippedArmor != null ? equippedArmor.defenseBonus : 0;
    }

    private void ForceRefreshUI()
    {
        if (UIManager.Instance != null)
        {
            UIManager.Instance.RefreshEquipment();
            UIManager.Instance.RefreshInventory();
            UIManager.Instance.RefreshAll();
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