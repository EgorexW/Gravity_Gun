using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class TargetsManager : MonoBehaviour{
    [BoxGroup("References")][Required][SerializeField] GameObject targetPrefab;
    [BoxGroup("References")] [Required] [SerializeField] Collider2D spawnBounds;
    
    [SerializeField] int targetCount = 10;
    [SerializeField] float collisionCheckSize = 2f;

List<Target> targets = new List<Target>();

    [FoldoutGroup("Events")] public UnityEvent onTargetDestroyed = new();

    void Awake(){
        UpdateTargets();
    }

    void UpdateTargets(){
        while (targets.Count < targetCount){
            CreateTarget();
        }
    }

    void CreateTarget(){
        var position = SelectTargetPos();
        var obj = Instantiate(targetPrefab, position, Quaternion.identity, transform);
        var target = obj.GetComponent<Target>();
        targets.Add(target);
        target.onDestroy.AddListener(OnTargetDestroyed);
    }

    void OnTargetDestroyed(Target target){
        targets.Remove(target);
        UpdateTargets();
        onTargetDestroyed.Invoke();
    }

    Vector2 SelectTargetPos(){
        for (var i = 0; i < General.Iterationlimit; i++){
            var pos = General.RandomPointInsideCollider2D(spawnBounds);
            var collider = Physics2D.OverlapCircle(pos, collisionCheckSize);
            if (collider != null){
                // Debug.Log(collider, collider);
                Debug.DrawLine(pos, collider.transform.position, Color.red, 1f);
                continue;
            }
            return pos;
        }
        return Vector2.zero;
    }
}