using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInterfaceController : MonoBehaviour
{
	public Image m_healthBar;
	public Image m_gunnerAmmoBar;
	public Image m_runnerAmmoBar;

	public WeaponCompositionDisplay m_weaponCompositionDisplay;
	public MovementCompositionDisplay m_movementCompostionDisplay;

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
		UpdateBarDisplay(m_healthBar, m_health.m_currentHealth / m_health.m_maxHealth);
		UpdateBarDisplay(m_gunnerAmmoBar, (float)m_shootController.m_ammoCount / (float)m_shootController.m_currentWeaponComposition.m_shotPattern.m_ammoCount);
		UpdateBarDisplay(m_runnerAmmoBar, (float)m_player.m_movementAbilityAmmoCount / (float)m_player.m_currentMovementAbilityComposition.m_movementType.m_ammoCount);

		m_weaponCompositionDisplay.UpdateDisplay(m_shootController.m_currentWeaponComposition);
		m_movementCompostionDisplay.UpdateDisplay(m_player.m_currentMovementAbilityComposition);
	}

	private void UpdateBarDisplay(Image p_targetImage, float p_displayValue)
	{
		p_targetImage.fillAmount = p_displayValue;
	}

	[System.Serializable]
	public struct WeaponCompositionDisplay
	{
		//public Image m_shotPatternImage;
		//public Image m_bulletTypeImage;
		public Image m_damageTypeImage;

		public void UpdateDisplay(ShootController.WeaponComposition p_weaponComposition)
		{
			//m_shotPatternImage.sprite = p_weaponComposition.m_shotPattern.m_uiSprite;
			//m_bulletTypeImage.sprite = p_weaponComposition.m_bulletType.m_uiSprite;
			m_damageTypeImage.sprite = p_weaponComposition.m_damageType.m_uiSprite;
		}
	}

	[System.Serializable]
	public struct MovementCompositionDisplay
	{
		//public Image m_movementTypeImage;
		public Image m_trailTypeImage;
		//public Image m_buffTypeImage;

		public void UpdateDisplay(PlayerController.MovementAbilityComposition p_movementAbilityCompostion)
		{
			//m_movementTypeImage.sprite = p_movementAbilityCompostion.m_movementType.m_uiSprite;
			m_trailTypeImage.sprite = p_movementAbilityCompostion.m_trailType.m_uiSprite;
			//m_buffTypeImage.sprite = p_movementAbilityCompostion.m_buffType.m_uiSprite;
		}
	}
}
