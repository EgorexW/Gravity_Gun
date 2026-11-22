using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class Damageable : MonoBehaviour
{
    [SerializeField] float health = 1;

    [FoldoutGroup("Events")] public UnityEvent onDie = new();

    public void Damage(float amount){
        health -= amount;
        if (health <= 0){
            Die();
        }
    }

    void Die(){
        Destroy(gameObject);
        onDie.Invoke();
    }
}
