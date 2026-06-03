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

    [SerializeField] private float chaseRange = 8f;
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

    private float idleTimer;
    private EnemyState currentState;

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

    void Update()
    {
        currentState = EnemyState.Idle;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= chaseRange)
        {
            idleCounter = 0f;
            currentState = EnemyState.Chase;
        }
        else
        {

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

    void Chase()
    {

    }
}