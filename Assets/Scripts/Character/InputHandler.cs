using Nrjwolf.Tools.AttachAttributes;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerInput))]
public class InputHandler : MonoBehaviour
{
    [GetComponent] [SerializeField] PlayerInput playerInput;

    [FormerlySerializedAs("characterMovement")] [BoxGroup("References")] [Required] [SerializeField] Movement movement;
    [BoxGroup("References")] [Required] [SerializeField] Looking looking;
    [BoxGroup("References")] [Required] [SerializeField] Gun gun; // TODO should't reference gun personally

    void Awake()
    {
        playerInput.onActionTriggered += OnActionTriggered;
    }

    void OnActionTriggered(InputAction.CallbackContext obj)
    {
        if (obj.action.actionMap.name != "Player"){
            return;
        }
        switch (obj.action.name){
            case "Move":
                OnMovePerformed(obj);
                break;
            case "Target":
                OnTargetPerformed(obj);
                break;
            case "Shoot":
                OnShootPerformed(obj);
                break;
            default:
                Debug.LogWarning("Unhandled action: " + obj.action.name);
                break;
        }
    }

    void OnShootPerformed(InputAction.CallbackContext obj){
        gun.constantFire = obj.performed;
    }

    void OnTargetPerformed(InputAction.CallbackContext obj)
    {
        var lookPoint = obj.ReadValue<Vector2>();
        lookPoint = General.GetMouseWorldPos(lookPoint);
        looking.SetTarget(lookPoint);
    }

    void OnMovePerformed(InputAction.CallbackContext obj)
    {
        var inputVector = obj.ReadValue<Vector2>();
        movement.SetMovementInput(inputVector);
    }
}