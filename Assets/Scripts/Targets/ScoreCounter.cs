using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

public class ScoreCounter : MonoBehaviour{
    [ShowInInspector] float score;

    public float Score => score;

    [FoldoutGroup("Events")] public UnityEvent<float> onScoreChange = new();

    public void IncrementScore(){
        score += 1;
        onScoreChange.Invoke(score);
    }
}