using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class PlayerInput : MonoBehaviour
{
	public int m_playerId = 0;

	private PlayerController m_playerController;
	private Player m_playerInputController;

	void Start()
	{
		m_playerInputController = ReInput.players.GetPlayer(m_playerId);
		m_playerController = GetComponent<PlayerController>();
	}

	void Update()
	{
		HandlePlayerInput();
	}

	void HandlePlayerInput()
	{
		Vector2 directionalInput = new Vector2(m_playerInputController.GetAxisRaw("Move Horizontal"), m_playerInputController.GetAxisRaw("Move Vertical"));
		m_playerController.SetDirectionalInput(directionalInput);

		Vector2 mouseAimInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		m_playerController.SetAimInput(Camera.main.ScreenToWorldPoint(mouseAimInput) - transform.position);

		if (m_playerInputController.GetButtonDown("Jump"))
		{
			m_playerController.OnJumpInputDown();
		}
		if (m_playerInputController.GetButtonUp("Jump"))
		{
			m_playerController.OnJumpInputUp();
		}

		if (m_playerInputController.GetButton("Shoot"))
		{
			m_playerController.OnShootInputHold();
		}

		if (m_playerInputController.GetButtonDown("Reload"))
		{
			m_playerController.OnReloadInputDown();
		}

		if (m_playerInputController.GetButtonDown("Movement Ability"))
		{
			m_playerController.OnMovementAbilityInputDown();
		}
	}
}
