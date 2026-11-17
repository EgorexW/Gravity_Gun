using UnityEngine;

public class Damageable : MonoBehaviour // TODO base interface
{
    [SerializeField] float health = 1;

    public void Damage(float amount){
        health -= amount;
        if (health <= 0){
            Destroy(gameObject);
        }
    }
}
