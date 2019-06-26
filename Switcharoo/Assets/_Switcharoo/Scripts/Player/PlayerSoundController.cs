
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class PlayerSoundController : MonoBehaviour
{
	[Header("Shoot Sounds")]
	public StudioEventEmitter m_ballisticShotSound;
	public StudioEventEmitter m_energyShotSound;

	private PlayerController m_player;

	private void Start()
	{
		m_player = GetComponentInParent<PlayerController>();
	}

	public void PlayShotSound()
	{
		for (int i = 0; i < m_player.m_players.Length; i++)
		{
			if (m_player.m_players[i].m_currentRole == PlayerController.PlayerRole.Gunner)
			{
				if (m_player.m_players[i].m_type == PlayerController.PlayerType.Robot)
				{
					m_ballisticShotSound.Play();
				}
				else if (m_player.m_players[i].m_type == PlayerController.PlayerType.Alien)
				{
					m_energyShotSound.Play();
				}
			}
		}
	}
}
