using System.Collections.Generic;
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

    private NavMeshObstacle obstacle;
    private bool isWaitingToEnableAgent;

    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float idleToPatrolTime = 3f;

    private Transform crumb;
    [SerializeField] private LayerMask breadcrumbMask;

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
    public float chaseRange = 12f;

    private LayerMask losMask;
    private Vector3 lastPlayerPosition;
    private float lastDestinationCalculation;

    private float idleTimer;

    [HideInInspector] public float distanceToPlayer;
    [HideInInspector] public EnemyState currentState;

    [HideInInspector] public bool isAttackPriority;
    [SerializeField] private float rotationSpeed = 2f;

    private void OnValidate()
    {
        chaseRange = Mathf.Round(chaseRange);
    }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        obstacle = GetComponent<NavMeshObstacle>();

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

        losMask = ~LayerMask.GetMask("Player", "Enemy", "Breadcrumb");

        breadcrumbMask = LayerMask.GetMask("Breadcrumb");

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

    private void FacePlayer()
    {
        ToggleAgent(false);

        Vector3 lookDirection = player.position - transform.position;

        Quaternion lookRot = Quaternion.LookRotation(lookDirection);

        transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * rotationSpeed);
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

    private void ToggleAgent(bool isOn)
    {
        if (isOn)
        {
            if (!agent.isActiveAndEnabled)
            {
                obstacle.enabled = false;
                isWaitingToEnableAgent = true;
            }
            else if (isWaitingToEnableAgent)
            {
                agent.enabled = true;
                isWaitingToEnableAgent = false;
            }
        }
        else
        {
            agent.enabled = false;
            obstacle.enabled = true;
            isWaitingToEnableAgent = false;
        }
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange && CheckLineOfSight(player))
        {
            if (distanceToPlayer <= attackRange)
            {
                if (isAttackPriority)
                {
                    currentState = EnemyState.Attack;
                }
                else
                {
                    currentState = EnemyState.Chase;
                }
            }
            else
            {
                currentState = EnemyState.Chase;
            }
        }
        else if (crumb != null)
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
    }

    private Transform GetBreadcrumb()
    {
        Collider[] crumbsInRange = Physics.OverlapSphere(
            transform.position,
            chaseRange,
            breadcrumbMask
        );

        if (crumbsInRange.Length == 0)
        {
            return null;
        }

        List<Transform> crumbsList = new List<Transform>();

        foreach (Collider crumb in crumbsInRange)
        {
            if (CheckLineOfSight(crumb.transform))
            {
                crumbsList.Add(crumb.transform);
            }
        }

        if (crumbsList.Count == 0)
        {
            return null;
        }

        crumbsList.Sort((a, b) =>
        {
            float lifespanA = a.GetComponent<Breadcrumb>().GetLifespan();
            float lifespanB = b.GetComponent<Breadcrumb>().GetLifespan();
            return lifespanB.CompareTo(lifespanA);
        });

        return crumbsList[0];
    }

    private void Idle()
    {
        ToggleAgent(false);

        idleCounter -= Time.deltaTime;

        if (idleCounter <= 0f)
        {
            idleCounter = 0f;

            idleTimeOut = Random.Range(idleDelay.x, idleDelay.y);
            idleCounter = idleTimeOut;

            patrolPosition = startPosition + Random.insideUnitSphere * patrolRadius;
            patrolPosition.y = startPosition.y;

            NavMeshHit hit;
            if (NavMesh.SamplePosition(transform.position, out hit, 1f, NavMesh.AllAreas))
            {
                Vector3 validStart = hit.position;
                NavMeshPath path = new NavMeshPath();

                if (NavMesh.CalculatePath(validStart, patrolPosition, NavMesh.AllAreas, path) &&
                    path.status == NavMeshPathStatus.PathComplete)
                {
                    currentState = EnemyState.Patrol;
                }
                else
                {
                    patrolPosition = startPosition + Random.insideUnitSphere * patrolRadius;
                    patrolPosition.y = startPosition.y;
                }
            }
        }
    }

    private void Patrol()
    {
        ToggleAgent(true);

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

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
        ToggleAgent(false);
        FacePlayer();

    }

    private void Chase()
    {
        ToggleAgent(true);

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh)
            return;

        idleCounter = idleTimeOut;

        if (distanceToPlayer <= chaseRange && CheckLineOfSight(player))
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
        else if (crumb != null)
        {
            if (agent.destination != crumb.position)
            {
                agent.stoppingDistance = 0f;
                agent.SetDestination(crumb.position);
            }

            float distanceToCrumb = Vector3.Distance(transform.position, crumb.position);
            if (distanceToCrumb <= 0.5f)
            {
                Destroy(crumb.gameObject);
                crumb = null;
            }
        }
    }
}