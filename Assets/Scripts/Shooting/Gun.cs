using System;
using Sirenix.OdinInspector;
using UnityEngine;

public class Gun : MonoBehaviour{
    [BoxGroup("References")][Required][SerializeField] GameObject bulletPrefab;
    
    [SerializeField] float cooldown = 1f;

    float lastShootTime = 0f;
    
    public bool constantFire = false;

    void Update(){
        if (constantFire){
            Shoot();
        }
    }

    public void Shoot(){
        if (lastShootTime + cooldown > Time.time){
            return;
        }
        lastShootTime  = Time.time;
        
        Instantiate(bulletPrefab, transform.position, transform.rotation);
    }
}
