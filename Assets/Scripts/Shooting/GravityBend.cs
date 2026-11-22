using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class GravityBend : MonoBehaviour{
    static int BulletsLayer => LayerMask.GetMask("Bullets");
    
    [SerializeField] float strength = 2;
    [SerializeField] float range = 5;

    public bool active;
    public Vector2 pos;

    [FoldoutGroup("Events")] public UnityEvent<Vector2> onHold = new();

    void FixedUpdate(){
        if (active){
            Hold();
        }
    }

    void Hold(){
        var bullets = Physics2D.OverlapCircleAll(pos, range, BulletsLayer);

        foreach (var bulletCollider in bullets){
            var bullet = bulletCollider.gameObject.GetComponent<Bullet>();
            if (bullet == null){
                Debug.LogWarning("Found obj on Bullet Layer with no Bullet component", bulletCollider);
                continue;
            }
            var dir = pos - (Vector2)bullet.transform.position;
            var mult = 1 - Mathf.InverseLerp(0,  range, dir.magnitude);
            var pullValue = strength * mult * Time.fixedDeltaTime;
            var pull = pullValue * dir.normalized;
                
            bullet.Pull(pull);
        }
        
        onHold.Invoke(pos);
    }

}