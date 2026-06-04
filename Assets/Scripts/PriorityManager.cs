using System.Collections.Generic;
using UnityEngine;

public class PriorityManager : MonoBehaviour
{
    [SerializeField] private int maxAttackers = 3;

    private List<EnemyAI> enemyList = new List<EnemyAI>();
    private List<EnemyAI> priorityList = new List<EnemyAI>();

    private void Awake()
    {
        EnemyAI[] enemies = FindObjectsOfType<EnemyAI>();
        foreach (EnemyAI enemy in enemies)
        {
            enemyList.Add(enemy);
        }
    }

    private void Update()
    {
        priorityList.Clear();

        foreach (EnemyAI enemy in enemyList)
        {
            enemy.isAttackPriority = false;

            if (enemy.distanceToPlayer <= enemy.chaseRange)
            {
                priorityList.Add(enemy);
            }
        }

        priorityList.Sort((a, b) => a.distanceToPlayer.CompareTo(b.distanceToPlayer));

        for (int i = 0; i < priorityList.Count; i++)
        {
            if (i < maxAttackers)
            {
                priorityList[i].isAttackPriority = true;
            }
        }
    }
}