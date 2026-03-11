using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPlayerTurn { get; private set; } = true;
    public bool IsActionProcessing { get; private set; } = false;

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
        return IsPlayerTurn && !IsActionProcessing;
    }

    public void BeginPlayerAction()
    {
        IsActionProcessing = true;
    }

    public void EndPlayerAction()
    {
        StartCoroutine(ProcessEnemyTurn());
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

            yield return enemy.TakeTurn(player);
        }

        IsActionProcessing = false;
        IsPlayerTurn = true;
    }
}