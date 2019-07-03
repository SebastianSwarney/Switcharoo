using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapUI : MonoBehaviour
{
    Coroutine m_colorFade;
    public Color m_startColor, m_endColorOrange, m_endColorBlue;
    public List<UnityEngine.UI.Image> m_images;
    public float m_blinkTime = 1f;


    [Header("Size Change")]
    Coroutine m_sizeChange;
    public GameObject m_targetGameObject;
    public Vector3 m_startScale, m_targetScale;
    
    public float m_sizeChangeTime = .25f;

    PlayerInput m_player;
    
    

    bool m_enable;
    public Color debugColor;
    private void Start()
    {
        m_player = DungeonManager.instance.m_playerGameObject.GetComponent<PlayerInput>();
    }
    private void Update()
    {
        debugColor = m_images[0].color;
    }
    public void ShowUI(bool p_enable)
    {
        
        m_enable = p_enable;
        
        if (m_sizeChange != null)
        {
            StopCoroutine(m_sizeChange);
            
            
        }
        if (m_colorFade != null)
        {
            StopCoroutine(m_colorFade);
        }
        if (p_enable)
        {
            m_sizeChange = StartCoroutine(StartChange());
        }
        else
        {
            m_sizeChange = StartCoroutine(EndChange());
        }
    }

    IEnumerator StartChange()
    {
        m_targetGameObject.SetActive(true);
        float percent = 0, currentTime = 0;
        m_images[0].color = m_startColor;
        
        
        m_targetGameObject.transform.localScale = m_startScale;

        while (percent <=1)
        {
            percent = currentTime / m_sizeChangeTime;
            m_images[0].color = Color.Lerp(Color.clear, m_startColor, percent);
            m_images[1].color = Color.Lerp(Color.clear, Color.white, percent);
            m_targetGameObject.transform.localScale = Vector3.Lerp(m_startScale, m_targetScale,percent);
            currentTime += Time.deltaTime;
            yield return null;
        }
        if (m_enable)
        {
            m_colorFade = StartCoroutine(ColorBlink());
        }
        m_sizeChange = null;
        
    }
    
    IEnumerator ColorBlink()
    {
        
        float percent = 0, currentTime =0;
        bool flip = false;
        Color endLerpColor = (m_player.m_humanSwap) ? m_endColorBlue : m_endColorOrange;
        while (true)
        {
            percent = currentTime / m_blinkTime;
            m_images[0].color = Color.Lerp((flip) ? endLerpColor : m_startColor, (flip) ? m_startColor : endLerpColor, percent);
            currentTime += Time.deltaTime;
            if(currentTime > m_blinkTime)
            {
                flip = !flip;
                currentTime = 0;
            }
            print("Running");
            yield return null;
        }
    }


    IEnumerator EndChange()
    {
        print("Oi");
        float percent = 0, currentTime = 0;

        Color startLerpColor = debugColor;
        
        m_targetGameObject.transform.localScale = m_targetScale;

        while (percent <= 1)
        {
            percent = currentTime / (m_sizeChangeTime);
            m_images[0].color = Color.Lerp(startLerpColor, Color.clear, percent);
            m_images[1].color = Color.Lerp(Color.white, Color.clear, percent);
            m_targetGameObject.transform.localScale = Vector3.Lerp(m_targetScale, Vector3.zero, percent);
            currentTime += Time.deltaTime;
            yield return null;
        }
        m_sizeChange = null;
        m_targetGameObject.SetActive(false);
    }
}
