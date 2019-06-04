using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceController : MonoBehaviour
{
	public Text m_runnerAmmoText;
	public Text m_gunnerAmmoText;

	public Text m_healthText;

	public PlayerController m_player;
	private ShootController m_shootController;
	private Health_Player m_health;

	private void Start()
	{
		m_shootController = m_player.gameObject.GetComponent<ShootController>();
		m_health = m_player.gameObject.GetComponent<Health_Player>();
	}

	private void Update()
	{
		UpdateAmmoText();
		UpdateHealthText();
	}

	private void UpdateAmmoText()
	{
		m_runnerAmmoText.text = m_player.m_movementAbilityAmmoCount.ToString();
		m_gunnerAmmoText.text = m_shootController.m_ammoCount.ToString();
	}

	private void UpdateHealthText()
	{
		m_healthText.text = m_health.m_currentHealth.ToString();
	}

}
