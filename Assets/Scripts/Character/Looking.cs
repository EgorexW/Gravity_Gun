using Sirenix.OdinInspector;
using UnityEngine;

public class Looking : MonoBehaviour{
    [BoxGroup("References")] [Required] [SerializeField] Rigidbody2D rb;
    Vector2 lookPoint;

    void FixedUpdate(){
        UpdateRotation();
    }

    public void SetTarget(Vector2 newLookPoint){
        lookPoint = newLookPoint;
    }

    void UpdateRotation(){
        var lookDir = lookPoint - (Vector2)transform.position;
        rb.transform.rotation = Quaternion.LookRotation(Vector3.forward, lookDir);
    }
}