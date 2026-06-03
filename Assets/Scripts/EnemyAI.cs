using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public enum EnemyState
    {
        Idle,
        Patrol,
        Chase,
        Attack
    }

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float idleToPatrolTime = 3f;

    private Transform player;
    private NavMeshAgent agent;
    private Animator anim;

    [SerializeField] private float patrolRadius = 5f;
    [SerializeField] private Vector2 idleDelay = new Vector2(3f, 8f);

    private Vector3 startPosition;
    private Vector3 patrolPosition;
    private float idleTimeOut;
    private float idleCounter;

    [Header("Chase Settings")]
    [SerializeField, Range(5f, 25f)]
    private float chaseRange = 12f;

    private LayerMask losMask;
    private float distanceToPlayer;
    private Vector3 lastPlayerPosition;
    private float lastDestinationCalculation;

    private float idleTimer;
    private EnemyState currentState;

    private void OnValidate()
    {
        chaseRange = Mathf.Round(chaseRange);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

        startPosition = transform.position;

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
        else
        {
            Debug.LogError("No se ha encontrado ningún objeto con la Tag 'Player'");
            enabled = false;
            return;
        }

        attackRange = player.GetComponent<NavMeshAgent>().radius + agent.radius + 0.5f;

        losMask = ~LayerMask.GetMask("Player", "Enemy");

        currentState = EnemyState.Idle;
        Idle();
        idleTimer = 0f;

        if (attackRange >= chaseRange)
        {
            Debug.LogWarning("attackRange debería ser menor que chaseRange.");
        }

        if (attackRange <= 0f || chaseRange <= 0f || idleToPatrolTime <= 0f)
        {
            Debug.LogWarning("Los rangos y tiempos deberían ser mayores que 0.");
        }
    }

    private bool CheckLineOfSight(Transform target)
    {
        Vector3 myPos = transform.position + Vector3.up;

        Vector3 targetPos = new Vector3(target.position.x, myPos.y, target.position.z);

        float rayDistance = Vector3.Distance(myPos, targetPos);
        Vector3 direction = (targetPos - myPos).normalized;

        if (Physics.Raycast(myPos, direction, out RaycastHit hit, rayDistance, losMask))
        {

            Debug.DrawRay(myPos, direction * hit.distance, Color.red);
            return false;
        }
        else
        {

            Debug.DrawRay(myPos, direction * rayDistance, Color.green);
            return true;
        }
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && CheckLineOfSight(player))
        {
            currentState = EnemyState.Chase;
        }
        else
        {
            currentState = EnemyState.Idle;
        }

        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Patrol:
                Patrol();
                break;
            case EnemyState.Chase:
                Chase();
                break;
            case EnemyState.Attack:
                Attack();
                break;
        }
        Debug.Log("Estado del enemigo: " + currentState);
    }

    private void Idle()
    {

        idleCounter -= Time.deltaTime;

        if (idleCounter <= 0f)
        {

            idleCounter = 0f;

            idleTimeOut = Random.Range(idleDelay.x, idleDelay.y);
            idleCounter = idleTimeOut;

            patrolPosition = startPosition + Random.insideUnitSphere * patrolRadius;

            patrolPosition.y = startPosition.y;

            currentState = EnemyState.Patrol;
        }
    }

    private void Patrol()
    {

        agent.stoppingDistance = 0f;

        if (agent.destination != patrolPosition)
        {
            agent.SetDestination(patrolPosition);


            if (agent.pathStatus != NavMeshPathStatus.PathComplete)
            {

                agent.SetDestination(transform.position);
                patrolPosition = agent.destination;
                return;
            }
        }

        float remainingDistance = Vector3.Distance(transform.position, patrolPosition);


        if (remainingDistance <= 0.1f)
        {
            idleCounter = 0f;
            currentState = EnemyState.Idle;
        }
    }

    void Attack()
    {

    }

    private void Chase()
    {
        idleCounter = idleTimeOut;

        if (CheckLineOfSight(player))
        {

            float timeSinceLastCalc = Time.time - lastDestinationCalculation;
            float distanceMovedByPlayer = Vector3.Distance(player.position, lastPlayerPosition);

            if (distanceMovedByPlayer > 0.5f || timeSinceLastCalc > 0.5f)
            {
                agent.stoppingDistance = attackRange;
                agent.SetDestination(player.position);

                lastPlayerPosition = player.position;
                lastDestinationCalculation = Time.time;
            }
        }
    }
}