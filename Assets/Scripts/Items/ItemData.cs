using UnityEngine;

[CreateAssetMenu(fileName = "NewItemData", menuName = "PixelDungeon/Item Data")]
public class ItemData : ScriptableObject
{
    [Header("Basic Info")]
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType;

    [Header("Visual")]
    public Sprite icon;

    [Header("Consumable")]
    public int healAmount = 0;

    [Header("Equipment")]
    public int attackBonus = 0;
    public int defenseBonus = 0;
}