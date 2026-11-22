using Sirenix.OdinInspector;
using UnityEngine;

public class Movement : MonoBehaviour{
    [BoxGroup("References")] [Required] [SerializeField] Rigidbody2D rb;

    [SerializeField] float speed = 5f;

    public Vector2 MovementInput{ get; private set; }

    void FixedUpdate(){
        UpdatePosition();
    }

    public void SetMovementInput(Vector2 inputVector){
        MovementInput = inputVector.normalized;
    }

    void UpdatePosition(){
        var move = MovementInput * (speed * Time.fixedDeltaTime);
        rb.MovePosition(rb.position + move);
    }
}