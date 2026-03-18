using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;
    public bool IsActionProcessing { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;
    public bool IsGameClear { get; private set; } = false;

    private readonly List<EnemyController> enemies = new List<EnemyController>();
    private PlayerController player;

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
        player = FindAnyObjectByType<PlayerController>();
    }

    public bool CanPlayerAct()
    {
        if (IsGameOver || IsGameClear)
            return false;

        return IsPlayerTurn && !IsActionProcessing;
    }

    public void BeginPlayerAction()
    {
        if (IsGameOver || IsGameClear)
            return;

        IsActionProcessing = true;
    }

    public void EndPlayerAction()
    {
        if (IsGameOver || IsGameClear)
            return;

        StartCoroutine(ProcessEnemyTurn());
    }

    public void ForcePlayerTurnReady()
    {
        StopAllCoroutines();
        IsActionProcessing = false;
        IsPlayerTurn = true;
    }

    public void RegisterEnemy(EnemyController enemy)
    {
        if (enemy == null)
            return;

        if (!enemies.Contains(enemy))
        {
            enemies.Add(enemy);
        }
    }

    public void UnregisterEnemy(EnemyController enemy)
    {
        if (enemy == null)
            return;

        enemies.Remove(enemy);
    }

    public void ClearAllEnemies()
    {
        enemies.Clear();
    }

    public void TriggerGameOver()
    {
        if (IsGameOver)
            return;

        IsGameOver = true;
        IsActionProcessing = false;
        IsPlayerTurn = false;

        MessageLog.Instance?.AddMessage("게임 오버");
        UIManager.Instance?.ShowGameOverPanel();
    }

    public void TriggerGameClear()
    {
        if (IsGameClear)
            return;

        IsGameClear = true;
        IsActionProcessing = false;
        IsPlayerTurn = false;

        MessageLog.Instance?.AddMessage("데모 클리어");
        UIManager.Instance?.ShowClearPanel();
    }

    private IEnumerator ProcessEnemyTurn()
    {
        IsPlayerTurn = false;

        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i] == null)
            {
                enemies.RemoveAt(i);
            }
        }

        foreach (EnemyController enemy in enemies)
        {
            if (enemy == null)
                continue;

            if (IsGameOver || IsGameClear)
                yield break;

            yield return enemy.TakeTurn(player);
        }

        IsActionProcessing = false;
        IsPlayerTurn = true;
    }
}