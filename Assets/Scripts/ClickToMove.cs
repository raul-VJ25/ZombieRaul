using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    [SerializeField] private GameObject clickTarget;

    private RaycastHit hit = new RaycastHit();
    private NavMeshAgent agent;
    private Transform tempContainer;
    private Camera cam;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        cam = Camera.main;
        tempContainer = GameObject.Find("TempContainer").transform;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject goClick = Instantiate(
                    clickTarget,
                    hit.point,
                    Quaternion.identity,
                    tempContainer
                );

                goClick.name = "Click Target";
                agent.SetDestination(hit.point);
            }
        }
    }
}