using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;
    public bool IsActionProcessing { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("GameManager initialized.");
        }
        else
        {
            Debug.LogWarning("Duplicate GameManager found. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    public bool CanPlayerAct()
    {
        return IsPlayerTurn && !IsActionProcessing;
    }

    public void BeginPlayerAction()
    {
        IsActionProcessing = true;
        Debug.Log("Player action started.");
    }

    public void EndPlayerAction()
    {
        Debug.Log("Player Turn End");
        IsActionProcessing = false;
        IsPlayerTurn = true;
    }
}