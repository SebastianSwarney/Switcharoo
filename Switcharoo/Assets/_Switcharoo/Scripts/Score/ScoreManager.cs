using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
   public static ScoreManager Instance { get; private set; }

    public int m_playerScore;
    private void Awake()
    {

        Instance = this;
    }

    public void AddScore(int m_scoreAmount)
    {
        m_playerScore += m_scoreAmount;
    }
}
