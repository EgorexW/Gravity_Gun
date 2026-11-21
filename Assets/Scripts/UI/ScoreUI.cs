
using System;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;

public class ScoreUI : UIElement{
    [BoxGroup("References")][Required][SerializeField] ScoreCounter score;
    [BoxGroup("References")][Required][SerializeField] TextMeshProUGUI scoreText;

    void Awake(){
        score.onScoreChange.AddListener(OnScoreChange);
    }

    void OnScoreChange(float arg0){
        scoreText.text = score.Score.ToString();
    }
}