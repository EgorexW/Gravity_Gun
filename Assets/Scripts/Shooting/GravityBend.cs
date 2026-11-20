using System;
using UnityEngine;

public class GravityBend : MonoBehaviour{
    [SerializeField] float strength = 2;
    [SerializeField] float range = 5;

    public bool active;

    void Update(){
        if (active){
            Hold(General.GetMouseWorldPos()); //TODO This is temporary
        }
    }

    public void Hold(Vector2 pos){
        var bullets = FindObjectsByType<Bullet>(FindObjectsSortMode.None); //TODO This is temporary

        foreach (var bullet in bullets){
            var dir = pos - (Vector2)bullet.transform.position;
            if (dir.sqrMagnitude > range * range){
                continue;
            }
            var mult = Mathf.InverseLerp(0,  range, dir.magnitude);
            var pullValue = strength * mult * Time.deltaTime;
            var pull = pullValue * dir.normalized;
                
            bullet.Pull(pull);
        }
    }
}