using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorFade : MonoBehaviour
{
    float m_currentTime;


    public List<ColorChange> m_changeColors;


    private void OnEnable()
    {
        m_currentTime = 0;
    }
    private void OnDisable()
    {
        foreach (ColorChange currentItem in m_changeColors)
        {
            currentItem.ChangeColor(0);
        }
    }
    private void Update()
    {
        m_currentTime += Time.deltaTime;
        foreach (ColorChange currentItem in m_changeColors)
        {
            currentItem.ChangeColor(m_currentTime);
        }
    }

    [System.Serializable]
    public struct ColorChange
    {
        public Text m_textAsset;
        public Image m_imageAsset;
        public SpriteRenderer m_spriteAsset;
        [Header("Colors")]
        public Color m_startColor;
        public Color m_endColor;
        public float m_transitionTime;

        public void ChangeColor(float p_currentTime)
        {
            if (p_currentTime / m_transitionTime <= 1)
            {


                if (m_textAsset != null)
                {
                    m_textAsset.color = Color.Lerp(m_startColor, m_endColor, p_currentTime / m_transitionTime);
                }
                if (m_imageAsset != null)
                {
                    m_imageAsset.color = Color.Lerp(m_startColor, m_endColor, p_currentTime / m_transitionTime);
                }
                if (m_spriteAsset != null)
                {
                    m_spriteAsset.color = Color.Lerp(m_startColor, m_endColor, p_currentTime / m_transitionTime);
                }
            }
        }
    }

}
