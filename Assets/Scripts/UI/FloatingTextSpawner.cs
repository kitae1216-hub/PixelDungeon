using UnityEngine;

public class FloatingTextSpawner : MonoBehaviour
{
    public static FloatingTextSpawner Instance;

    [SerializeField] private FloatingText floatingTextPrefab;

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

    public void SpawnText(Vector3 worldPosition, string text, Color color)
    {
        if (floatingTextPrefab == null)
            return;

        FloatingText popup = Instantiate(floatingTextPrefab, worldPosition, Quaternion.identity);
        popup.Initialize(text, color);
    }
}