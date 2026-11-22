using Sirenix.OdinInspector;
using UnityEngine;

public class GravityBendVisuals : MonoBehaviour{
    [BoxGroup("References")] [Required] [SerializeField] GravityBend gravityBend;

    [BoxGroup("References")] [Required] [SerializeField] GameObject visualObject;

    void Awake(){
        gravityBend.onHold.AddListener(HoldVisuals);
    }

    void Update(){
        visualObject.SetActive(gravityBend.active);
    }

    void HoldVisuals(Vector2 arg0){
        visualObject.transform.position = arg0;
    }
}