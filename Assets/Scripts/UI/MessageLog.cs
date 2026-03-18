using System.Collections.Generic;
using UnityEngine;

public class MessageLog : MonoBehaviour
{
    public static MessageLog Instance;

    [SerializeField] private int maxMessages = 5;

    private readonly Queue<string> messages = new Queue<string>();

    public IReadOnlyCollection<string> Messages => messages;

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

    public void AddMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return;

        messages.Enqueue(message);

        while (messages.Count > maxMessages)
        {
            messages.Dequeue();
        }

        UIManager.Instance?.RefreshLog();
    }

    public string GetCombinedMessages()
    {
        return string.Join("\n", messages);
    }

    public void ClearLog()
    {
        messages.Clear();
        UIManager.Instance?.RefreshLog();
    }
}