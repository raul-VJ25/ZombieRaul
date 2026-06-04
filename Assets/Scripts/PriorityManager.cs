using System.Collections.Generic;
using UnityEngine;

public class PriorityManager : MonoBehaviour
{
    [SerializeField] private int maxAttackers = 3;

    private List<EnemyAI> enemyList = new List<EnemyAI>();
    private List<EnemyAI> priorityList = new List<EnemyAI>();

    private Transform player;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player").transform;

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

        priorityList.Sort((a, b) =>
        {
            float distA = Vector3.Distance(a.transform.position, player.position) - a.priorityBonus;
            float distB = Vector3.Distance(b.transform.position, player.position) - b.priorityBonus;
            return distA.CompareTo(distB);
        });

        for (int i = 0; i < priorityList.Count; i++)
        {
            if (i < maxAttackers)
            {
                priorityList[i].isAttackPriority = true;
            }
        }
    }
}