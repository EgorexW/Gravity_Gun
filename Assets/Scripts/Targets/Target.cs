using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

class Target : MonoBehaviour{
    [BoxGroup("References")][Required][SerializeField] Damageable damageable;
    
    [FoldoutGroup("Events")] public UnityEvent<Target> onDestroy = new();

    void Awake(){
        damageable.onDie.AddListener(OnDie);
    }

    void OnDie(){
        onDestroy.Invoke(this);
    }
}