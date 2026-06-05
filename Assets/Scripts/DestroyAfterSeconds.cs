using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    [SerializeField] private float seconds = 8f;

    private void Start()
    {
        Destroy(gameObject, seconds);
    }
}