using UnityEngine;

public class Health : MonoBehaviour
{
    public void ChangeHealth(float amount)
    {
        Debug.Log(gameObject.name + " received " + amount + " damage.");
    }
}