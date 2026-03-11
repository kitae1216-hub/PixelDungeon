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
    }

    public void TakeDamage(int amount)
    {
        if (IsDead)
            return;

        CurrentHP -= amount;
        CurrentHP = Mathf.Max(CurrentHP, 0);

        Debug.Log($"{gameObject.name} took {amount} damage. HP: {CurrentHP}/{maxHP}");

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
    }

    private void Die()
    {
        Debug.Log($"{gameObject.name} died.");

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

        if (destroyOnDeath)
        {
            Destroy(gameObject);
        }
    }
}