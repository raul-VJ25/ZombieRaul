using UnityEngine;
using UnityEngine.AI;

public class BreadcrumbGenerator : MonoBehaviour
{
    [SerializeField] private GameObject breadcrumbPrefab;
    [SerializeField] private float breadcrumbInterval = 0.2f;
    [SerializeField] private bool showBreadcrumbs = true;

    private NavMeshAgent agent;
    private Transform tempContainer;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();

        GameObject container = GameObject.Find("Temp Container");
        if (container == null)
        {
            container = new GameObject("Temp Container");
        }
        tempContainer = container.transform;

        InvokeRepeating(nameof(GenerateBreadcrumb), 0f, breadcrumbInterval);
    }

    private void GenerateBreadcrumb()
    {
        if (agent.velocity.magnitude > 0.1f)
        {
            GameObject crumb = Instantiate(
                breadcrumbPrefab,
                transform.position,
                Quaternion.identity,
                tempContainer
            );

            if (!showBreadcrumbs)
            {
                crumb.SetActive(false);
            }
        }
    }
}