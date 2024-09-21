using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ScoreManager 
{
    public float curScore;
    public float maxScore;

    public void AddScore(float score)
    {
        curScore += score;
        UIManager.Instance.SetScoreSliderValue(curScore, maxScore);
    }

    public void ResetScore()
    {
        curScore = 0;
        UIManager.Instance.SetScoreSliderValue(curScore, maxScore);
    }

    public void SetMaxScore(float maxScore)
    {
        this.maxScore = maxScore;
    }
}
