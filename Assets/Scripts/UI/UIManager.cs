using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("References")]
    [SerializeField] private PlayerController player;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Inventory inventory;
    [SerializeField] private EquipmentManager equipmentManager;

    [Header("Text UI")]
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private TextMeshProUGUI floorText;
    [SerializeField] private TextMeshProUGUI equipmentText;
    [SerializeField] private TextMeshProUGUI inventoryText;
    [SerializeField] private TextMeshProUGUI logText;

    [Header("Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject clearPanel;

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

    private void Start()
    {
        if (player == null)
            player = FindAnyObjectByType<PlayerController>();

        if (playerHealth == null && player != null)
            playerHealth = player.GetComponent<Health>();

        if (inventory == null && player != null)
            inventory = player.GetComponent<Inventory>();

        if (equipmentManager == null && player != null)
            equipmentManager = player.GetComponent<EquipmentManager>();

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (clearPanel != null)
            clearPanel.SetActive(false);

        RefreshAll();
    }

    private void Update()
    {
        RefreshAll();
    }

    public void RefreshAll()
    {
        RefreshHP();
        RefreshFloor();
        RefreshEquipment();
        RefreshInventory();
        RefreshLog();
    }

    public void RefreshHP()
    {
        if (hpText == null || playerHealth == null)
            return;

        hpText.text = $"HP: {playerHealth.CurrentHP} / {playerHealth.MaxHP}";
    }

    public void RefreshFloor()
    {
        if (floorText == null || FloorManager.Instance == null)
            return;

        floorText.text = $"Floor: {FloorManager.Instance.CurrentFloor} / {FloorManager.Instance.TargetFloor}";
    }

    public void RefreshEquipment()
    {
        if (equipmentText == null || equipmentManager == null)
            return;

        string weapon = equipmentManager.EquippedWeapon != null ? equipmentManager.EquippedWeapon.itemName : "없음";
        string armor = equipmentManager.EquippedArmor != null ? equipmentManager.EquippedArmor.itemName : "없음";

        equipmentText.text =
            $"무기: {weapon}\n" +
            $"방어구: {armor}\n" +
            $"ATK 보너스: {equipmentManager.GetAttackBonus()}\n" +
            $"DEF 보너스: {equipmentManager.GetDefenseBonus()}";
    }

    public void RefreshInventory()
    {
        if (inventoryText == null || inventory == null)
            return;

        string result = "인벤토리\n";

        for (int i = 0; i < inventory.MaxSlots; i++)
        {
            ItemData item = inventory.GetItemAt(i);
            if (item == null)
            {
                result += $"{i + 1}. (비어있음)\n";
            }
            else
            {
                result += $"{i + 1}. {item.itemName}\n";
            }
        }

        inventoryText.text = result;
    }

    public void RefreshLog()
    {
        if (logText == null || MessageLog.Instance == null)
            return;

        logText.text = MessageLog.Instance.GetCombinedMessages();
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void ShowClearPanel()
    {
        if (clearPanel != null)
            clearPanel.SetActive(true);
    }
}