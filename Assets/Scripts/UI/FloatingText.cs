using TMPro;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.8f;

    private TextMeshPro textMesh;
    private float timer;

    private void Awake()
    {
        textMesh = GetComponent<TextMeshPro>();
    }

    public void Initialize(string text, Color color)
    {
        if (textMesh != null)
        {
            textMesh.text = text;
            textMesh.color = color;
        }
    }

    private void Update()
    {
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
    }
}