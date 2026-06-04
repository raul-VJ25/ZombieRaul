using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Health : MonoBehaviour
{

    [SerializeField] private float health;
    [SerializeField] private float healthMax = 100f;
    [HideInInspector] public bool isDead;
    private bool isPlayer;
    private Animator anim;

    private void Awake()
    {
        health = healthMax;
        anim = GetComponent<Animator>();
        isPlayer = gameObject.CompareTag("Player");
    }

    public void ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, healthMax);

        if (health <= 0f && !isDead)
        {
            StartCoroutine(DeathHandler());
        }
    }

    private IEnumerator DeathHandler()
    {
        isDead = true;

        if (anim != null)
        {
            anim.SetBool("move", false);
            anim.SetBool("Attack", false);
            anim.SetBool("StandBy", false);
        }

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        LocomotionSimpleAgent locomotion = GetComponent<LocomotionSimpleAgent>();
        if (locomotion != null) locomotion.enabled = false;

        CapsuleCollider col = GetComponent<CapsuleCollider>();
        if (col != null) col.enabled = false;

        if (isPlayer)
        {
            Combat combat = GetComponent<Combat>();
            if (combat != null) combat.enabled = false;

            ClickToMove ctm = GetComponent<ClickToMove>();
            if (ctm != null) ctm.enabled = false;
        }
        else
        {
            EnemyAI enemyAI = GetComponent<EnemyAI>();
            if (enemyAI != null) enemyAI.enabled = false;

            NavMeshObstacle obstacle = GetComponent<NavMeshObstacle>();
            if (obstacle != null) obstacle.enabled = false;

            PriorityManager pm = GameObject.FindGameObjectWithTag("GameController").GetComponent<PriorityManager>();
            if (pm != null && enemyAI != null)
            {
                pm.RemoveEnemyFromList(enemyAI);
            }
        }

        yield return new WaitForSeconds(3f);

        Rigidbody rb = gameObject.AddComponent<Rigidbody>();
        rb.useGravity = true;
        rb.linearDamping = 5f;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        yield return new WaitForSeconds(3f);

        if (isPlayer)
        {
            gameObject.SetActive(false);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}