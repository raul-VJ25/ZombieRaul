using UnityEngine;

public class Breadcrumb : MonoBehaviour
{
    [SerializeField] private float lifespan = 3f;

    private void Update()
    {
        lifespan -= Time.deltaTime;

        if (lifespan <= 0f)
        {
            Destroy(gameObject);
        }
    }

    public float GetLifespan()
    {
        return lifespan;
    }
}