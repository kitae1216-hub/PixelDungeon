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
        }
        else
        {
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
    }

    public void EndPlayerAction()
    {
        Debug.Log("Player Turn End");
        IsActionProcessing = false;

        // 1일차는 적 턴이 없으므로 플레이어 턴 유지
        IsPlayerTurn = true;
    }
}
