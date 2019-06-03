using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Score_IndividualScore : MonoBehaviour
{
    public int m_scoreAmount;

    public void AddScoreToPlayer()
    {
        ScoreManager.Instance.AddScore(m_scoreAmount);
    }
}
