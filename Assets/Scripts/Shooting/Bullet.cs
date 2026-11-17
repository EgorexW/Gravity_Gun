using System;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour{
    const float PASS_THROUGH_TIME = 0.05f;
    
    [BoxGroup("References")][Required][SerializeField] Movement movement;

    [SerializeField] int maxBounces = 2;
    [SerializeField] float damage = 1;
    [SerializeField] float lifetime = 10;
    
    int bounces = 0;
    float startTime;

    void Awake(){
        Destroy(gameObject, lifetime);
        startTime = Time.time;
    }

    void Start(){
        movement.SetMovementInput(transform.up);
    }

    // void OnTriggerEnter2D(Collider2D other){
    //     if (Time.time - startTime < PASS_THROUGH_TIME){
    //         return;
    //     }
    //     if (bounces >= maxBounces){
    //         Destroy(gameObject);
    //     }
    //     var newDirection = Vector2.Reflect(movement.MovementInput, other.transform.position - transform.position);
    //     movement.SetMovementInput(newDirection);
    //     bounces++;
    // }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - startTime < PASS_THROUGH_TIME){
            return;
        }
        var firstContact = collision.contacts[0]; // TODO edge cases with multiple contacts
        if (firstContact.collider.gameObject.TryGetComponent<Damageable>(out var damageable)){
            damageable.Damage(damage);
        }
        if (bounces >= maxBounces){
            Destroy(gameObject);
        }
        Debug.DrawRay(firstContact.point, firstContact.normal * 100, Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f), 10f);
        var newDirection = Vector2.Reflect(movement.MovementInput, firstContact.normal);
        movement.SetMovementInput(newDirection);
        bounces++;
    }
}
