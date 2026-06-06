using UnityEngine;
using UnityEngine.AI;

public class Combat : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    private LayerMask opponentMask;

    [Header("Attack Settings")]
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private Vector2 dmgRange = new Vector2(2f, 5f);

    private Animator anim;
    private NavMeshAgent agent;
    private Camera cam;

    private void Awake()
    {
        isPlayer = gameObject.CompareTag("Player");

        if (isPlayer)
        {
            opponentMask = LayerMask.GetMask("Enemy");

            anim = GetComponent<Animator>();
            agent = GetComponent<NavMeshAgent>();
            cam = Camera.main;
        }
        else
        {
            opponentMask = LayerMask.GetMask("Player");
        }
    }

    private void Update()
    {
        if (!isPlayer) return;
        anim.SetBool("Attack", Input.GetMouseButton(1));


        PlayerLookAtEnemy();
    }

    private void PlayerLookAtEnemy()
    {
        if (Input.GetMouseButton(1) && agent.velocity.magnitude < 0.05f)
        {
            Vector3 input = Input.mousePosition;

            Vector3 lookPoint = cam.ScreenToWorldPoint(new Vector3(input.x, input.y, 10f));

            lookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);

            Quaternion lookRot = Quaternion.LookRotation(lookPoint - transform.position);

            transform.rotation = Quaternion.Lerp(transform.rotation, lookRot, Time.deltaTime * 10f);
        }
    }

    public void ImpactEvent()
    {
        Vector3 offset = transform.forward * 0.5f + Vector3.up;
        Vector3 attackCenter = transform.position + offset;

        Vector3 wallCheckOrigin = transform.position + Vector3.up * 0.9f;

        LayerMask obstacleMask = ~(opponentMask | (1 << gameObject.layer));

        Collider[] hits = Physics.OverlapSphere(attackCenter, attackRadius, opponentMask);

        if (hits.Length > 0)
        {
            float damageAmount = Mathf.Round(Random.Range(dmgRange.x, dmgRange.y));

            foreach (Collider hit in hits)
            {
                Vector3 targetPos = hit.transform.position + Vector3.up * 0.9f;

                if (Physics.Linecast(wallCheckOrigin, targetPos, out RaycastHit wallHit, obstacleMask))
                {
                    if (wallHit.transform != hit.transform)
                    {
                        Debug.DrawLine(wallCheckOrigin, targetPos, Color.red);
                        continue;
                    }
                }

                Debug.DrawLine(wallCheckOrigin, targetPos, Color.green);

                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.ChangeHealth(-damageAmount);
                }
            }
        }
    }
}