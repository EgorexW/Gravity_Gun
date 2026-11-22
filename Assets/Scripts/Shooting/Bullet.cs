using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Bullet : MonoBehaviour{
    const float PASS_THROUGH_TIME = 0.05f;
    const float STOPPED_MAGNITUDE = 0.1f;
    
    [BoxGroup("References")][Required][SerializeField] Rigidbody2D rb;

    [FormerlySerializedAs("velocity")] [SerializeField] float speed = 10;
    [SerializeField] int maxBounces = 2;
    [SerializeField] float damage = 1;
    [SerializeField] float lifetime = 10;
    
    int bounces = 0;
    float startTime;
    // Vector2 dir;

    void Awake(){
        Destroy(gameObject, lifetime);
        startTime = Time.time;
    }

    void Start(){
        rb.linearVelocity = transform.up * speed;
    }

    void Update(){
        if (rb.linearVelocity.magnitude <= STOPPED_MAGNITUDE){
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (Time.time - startTime < PASS_THROUGH_TIME){
            return;
        }
        var avgNormal = Vector2.zero;
        bool damaged = false;
        foreach (var contact in collision.contacts){
            if (!damaged && contact.collider.gameObject.TryGetComponent<Damageable>(out var damageable)){
                damageable.Damage(damage);
                damaged = true;
            }
            avgNormal += contact.normal;
        }
        avgNormal.Normalize();
        if (bounces >= maxBounces){
            Destroy(gameObject);
            return;
        }
        if (collision.contactCount > 1){
        }
        var dir = Vector2.Reflect(-collision.relativeVelocity.normalized, avgNormal); // TODO still does not work perfectly on corners
        transform.up = dir;
        rb.linearVelocity = transform.up * speed;
        bounces++;
    }

    public void Pull(Vector2 pull){
        Debug.DrawRay(transform.position, pull * 100, Color.red, 0.1f);
        rb.linearVelocity = pull + rb.linearVelocity;
    }
}
