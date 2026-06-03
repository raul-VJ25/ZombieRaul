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

    private float idleTimer;
    private EnemyState currentState;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();

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

    private void Update()
    {

        currentState = EnemyState.Idle;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= attackRange)
        {
            currentState = EnemyState.Attack;
            idleTimer = 0f;
        }
        else if (distanceToPlayer <= chaseRange)
        {
            currentState = EnemyState.Chase;
            idleTimer = 0f;
        }
        else
        {
            idleTimer += Time.deltaTime;

            if (idleTimer >= idleToPatrolTime)
            {
                currentState = EnemyState.Patrol;
            }

            switch (currentState)
            {
                case EnemyState.Idle:
                    Idle();
                    break;
                case EnemyState.Patrol:
                    Patrol();
                    break;
                case EnemyState.Attack:
                    Attack();
                    break;
                case EnemyState.Chase:
                    Chase();
                    break;
            }
        } 

        Debug.Log("Estado del enemigo: " + currentState);
    }

    void Idle()
    {
       
    }

    void Patrol()
    {
       
    }

    void Attack()
    {
        
    }

    void Chase()
    {
        
    }
}