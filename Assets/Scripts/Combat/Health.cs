using UnityEngine;

public class Health : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHP = 10;
    [SerializeField] private bool destroyOnDeath = false;

    public int CurrentHP { get; private set; }
    public int MaxHP => maxHP;
    public bool IsDead => CurrentHP <= 0;

    private void Awake()
    {
        CurrentHP = maxHP;
    }

    public void Initialize(int newMaxHP)
    {
        maxHP = newMaxHP;
        CurrentHP = maxHP;
        UIManager.Instance?.RefreshHP();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        EquipmentManager equipment = GetComponent<EquipmentManager>();
        int defense = equipment != null ? equipment.GetDefenseBonus() : 0;
        int finalDamage = Mathf.Max(1, amount - defense);

        CurrentHP -= finalDamage;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        FloatingTextSpawner.Instance?.SpawnText(
            transform.position + Vector3.up * 0.5f,
            $"-{finalDamage}",
            Color.red
        );

        MessageLog.Instance?.AddMessage($"{gameObject.name} 피해 {finalDamage}");
        UIManager.Instance?.RefreshHP();

        if (CurrentHP <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (IsDead)
            return;

        CurrentHP += amount;
        CurrentHP = Mathf.Min(CurrentHP, maxHP);

        FloatingTextSpawner.Instance?.SpawnText(
            transform.position + Vector3.up * 0.5f,
            $"+{amount}",
            Color.green
        );

        MessageLog.Instance?.AddMessage($"{gameObject.name} 회복 {amount}");
        UIManager.Instance?.RefreshHP();
    }

    private void Die()
    {
        MessageLog.Instance?.AddMessage($"{gameObject.name} 사망");

        GridOccupancyManager occupancy = GridOccupancyManager.Instance;
        if (occupancy != null)
        {
            occupancy.RemoveOccupant(gameObject);
        }

        EnemyController enemy = GetComponent<EnemyController>();
        if (enemy != null)
        {
            GameManager.Instance?.UnregisterEnemy(enemy);
        }

        PlayerController player = GetComponent<PlayerController>();
        if (player != null)
        {
            GameManager.Instance?.TriggerGameOver();
        }

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}