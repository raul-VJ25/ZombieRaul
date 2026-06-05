using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class Health : MonoBehaviour
{

    [SerializeField] private GameObject bloodPrefab;

    [SerializeField] private GameObject feedbackTextPrefab;

    [SerializeField] private float regionPerSecond = 1f;
    [SerializeField] private float regionDistance = 8f;

    private LayerMask opponentMask;

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

        if (isPlayer)
        {
            opponentMask = LayerMask.GetMask("Enemy");
        }
        else
        {
            opponentMask = LayerMask.GetMask("Player");
        }

        InvokeRepeating(nameof(Regeneration), 1f, 1f);
    }

    private void Regeneration()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, regionDistance, opponentMask);

        if (hits.Length == 0 && health < healthMax)
        {
            ChangeHealth(regionPerSecond);
        }
    }

    public void ChangeHealth(float amount)
    {
        health = Mathf.Clamp(health + amount, 0f, healthMax);

        if (amount != 0 && feedbackTextPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 2f;
            GameObject feedbackGO = Instantiate(feedbackTextPrefab, spawnPos, Quaternion.identity);

            FeedbackText feedback = feedbackGO.GetComponent<FeedbackText>();
            if (feedback != null)
            {
                feedback.ChangeText(amount);
            }
        }

        if (amount < 0 && bloodPrefab != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.05f;
            Quaternion spawnRot = Quaternion.Euler(-90f, Random.Range(0f, 360f), 0f);

            Instantiate(bloodPrefab, spawnPos, spawnRot);
        }

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

            anim.SetInteger("DeathID", Random.Range(1, 13));
            anim.SetTrigger("Death");
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

        if (anim != null)
        {
            AnimatorStateInfo stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            float elapsed = 0f;

            while (stateInfo.tagHash != Animator.StringToHash("death") && elapsed < 5f)
            {
                elapsed += Time.deltaTime;
                stateInfo = anim.GetCurrentAnimatorStateInfo(0);
                yield return null;
            }

            if (stateInfo.tagHash == Animator.StringToHash("death"))
            {
                float remainingTime = stateInfo.length - stateInfo.normalizedTime;
                yield return new WaitForSeconds(remainingTime + 1f);
            }
        }
        else
        {
            yield return new WaitForSeconds(3f);
        }

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