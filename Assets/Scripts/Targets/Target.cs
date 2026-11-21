using System;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

class Target : MonoBehaviour{
    [FoldoutGroup("Events")] public UnityEvent<Target> onDestroy = new();

    void OnDestroy(){
        onDestroy.Invoke(this);
    }
}