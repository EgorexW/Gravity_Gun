using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

public class Bullet : MonoBehaviour{
    const float PASS_THROUGH_TIME = 0.05f;
    const float STOPPED_MAGNITUDE = 0.1f;

    [BoxGroup("References")] [Required] [SerializeField] Rigidbody2D rb;

    [FormerlySerializedAs("speed")] [FormerlySerializedAs("velocity")] [SerializeField] float startSpeed = 10;
    [SerializeField] int maxBounces = 2;
    [SerializeField] float damage = 1;
    [SerializeField] float lifetime = 10;

    int bounces;
    float startTime;
    // Vector2 dir;

    void Awake(){
        Destroy(gameObject, lifetime);
        startTime = Time.time;
    }

    void Start(){
        rb.linearVelocity = transform.up * startSpeed;
    }

    void Update(){
        transform.up = rb.linearVelocity.normalized;
        //     if (rb.linearVelocity.magnitude <= STOPPED_MAGNITUDE){
        //         Destroy(gameObject);
        //     }
    }

    void OnCollisionEnter2D(Collision2D collision){
        if (Time.time - startTime < PASS_THROUGH_TIME){
            return;
        }
        var avgNormal = Vector2.zero;
        var damaged = false;
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
        if (collision.contactCount > 1){ }
        var dir = Vector2.Reflect(-collision.relativeVelocity.normalized, avgNormal);
        rb.linearVelocity = dir.normalized * collision.relativeVelocity.magnitude;
        bounces++;
    }

    public void Pull(Vector2 pull){
        Debug.DrawRay(transform.position, pull * 100, Color.red, 0.1f);
        rb.linearVelocity = pull + rb.linearVelocity;
    }
}