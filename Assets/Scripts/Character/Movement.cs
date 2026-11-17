using Sirenix.OdinInspector;
using UnityEngine;

public class Movement : MonoBehaviour{
    [BoxGroup("References")] [Required] [SerializeField] Rigidbody2D rb;

    [SerializeField] float speed = 5f;
    
    Vector2 movement;
    
    public Vector2 MovementInput => movement;
    
    void FixedUpdate(){
        UpdatePosition();
    }
    
    public void SetMovementInput(Vector2 inputVector){
        movement = inputVector.normalized;
    }
    
    void UpdatePosition(){
        var move = movement * (speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + move);
    }
}