using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public interface IPauseable
{
	void SetPauseState(bool p_isPaused);
}

public class PauseMenuController : MonoBehaviour
{
	public static PauseMenuController instance;

	[SerializeField]
	private bool m_isPaused;
    [HideInInspector]
    public bool m_canPause;

	public Canvas m_pauseCanvans;

	public List<IPauseable> m_pauseables = new List<IPauseable>();
    public UnityEngine.EventSystems.EventSystem m_eventSystem;
    public GameObject m_pauseScreenButton;


	private void Awake()
	{
		if (instance == null)
		{
			instance = this;
		}
		else
		{
			Destroy(this);
		}
	}

	private void Start()
	{
		m_canPause = true;

		SetUnPause();
	}

	private void Update()
	{
        if (!m_canPause) return;
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			TogglePause();
		}
	}

	public void TogglePause()
	{
		m_isPaused = !m_isPaused;

		if (m_isPaused)
		{
            ResetPauseMenu();
            SetPause();
            
		}
		else
		{
			SetUnPause();
		}
	}

	private void SetPause()
	{
		m_pauseCanvans.enabled = true;

		foreach (IPauseable pauseable in m_pauseables)
		{
			pauseable.SetPauseState(true);
		}
	}

	private void SetUnPause()
	{
		m_pauseCanvans.enabled = false;

		foreach (IPauseable pauseable in m_pauseables)
		{
			pauseable.SetPauseState(false);
		}
	}


    private void ResetPauseMenu()
    {
        Transform pauseMenu = transform.GetChild(0);
        for (int i = 0; i < pauseMenu.childCount; i++)
        {
            pauseMenu.GetChild(i).gameObject.SetActive(false);
        }
        pauseMenu.GetChild(0).gameObject.SetActive(true);
        m_eventSystem.SetSelectedGameObject(m_pauseScreenButton);
    }
}
