using UnityEngine;

public class Combat : MonoBehaviour
{
    [SerializeField] private bool isPlayer;
    private LayerMask opponentMask;

    [Header("Attack Settings")]
    [SerializeField] private float attackRadius = 0.6f;
    [SerializeField] private Vector2 dmgRange = new Vector2(2f, 5f);

    private void Awake()
    {
        isPlayer = gameObject.CompareTag("Player");

        if (isPlayer)
        {
            opponentMask = LayerMask.GetMask("Enemy");
        }
        else
        {
            opponentMask = LayerMask.GetMask("Player");
        }
    }

    public void ImpactEvent()
    {
        Vector3 offset = transform.forward * 0.5f + Vector3.up;

        Collider[] hits = Physics.OverlapSphere(transform.position + offset, attackRadius, opponentMask);

        if (hits.Length > 0)
        {
            float damageAmount = Mathf.Round(Random.Range(dmgRange.x, dmgRange.y));

            foreach (Collider hit in hits)
            {
                Health health = hit.GetComponent<Health>();
                if (health != null)
                {
                    health.ChangeHealth(-damageAmount);
                }
            }
        }
    }
}