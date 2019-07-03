using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;
using UnityEngine.Events;


[System.Serializable]
public class SwapPressed : UnityEvent<bool> { }
public class PlayerInput : MonoBehaviour
{
	public enum PlayerOrder { ZeroRunnerOneGunner, ZeroGunnerOneGunner }
	[HideInInspector]
	public PlayerOrder m_currentPlayerOrder;

	public bool m_isKeyboard;

	public bool m_isSinglePlayerController;

	private PlayerController m_playerController;

	private Player m_playerInputController;

	private Player m_player0InputController;
	private Player m_player1InputController;

	private Vector2 m_lastRunnerInput;

    
	private bool m_runnerSwap;   
	private bool m_gunnerSwap;

    [HideInInspector]
    public bool m_humanSwap;
    public SwapPressed m_swapPressed = new SwapPressed();

	void Start()
	{
		m_playerInputController = ReInput.players.GetPlayer(0);

		m_player0InputController = ReInput.players.GetPlayer(0);
		m_player1InputController = ReInput.players.GetPlayer(1);

		m_playerController = GetComponent<PlayerController>();
	}

	void Update()
	{
		switch (m_playerController.m_states.m_inputState)
		{
			case PlayerController.InputState.InputEnabled:

				if (m_isKeyboard)
				{
					HandleSinglePlayerInput();
				}
				else
				{
					if (m_isSinglePlayerController)
					{
						HandleRunnerInput(m_player0InputController);
						HandleGunnerInput(m_player0InputController);
					}
					else
					{
						HandleTwoPlayerInput();
					}
				}

				break;

			case PlayerController.InputState.InputDisabled:



				break;
		}

	}

	private void HandleTwoPlayerInput()
	{
		switch (m_currentPlayerOrder)
		{
			case PlayerOrder.ZeroRunnerOneGunner:

				HandleRunnerInput(m_player0InputController);
				HandleGunnerInput(m_player1InputController);

				break;

			case PlayerOrder.ZeroGunnerOneGunner:

				HandleRunnerInput(m_player1InputController);
				HandleGunnerInput(m_player0InputController);

				break;
		}

		if (m_gunnerSwap && m_runnerSwap)
		{
			m_gunnerSwap = false;
			m_runnerSwap = false;
            m_swapPressed.Invoke(false);


            m_playerController.OnReloadInputDown();
		}
	}

	#region Single Player Input
	private void HandleSinglePlayerInput()
	{
		Vector2 directionalInput = new Vector2(m_playerInputController.GetAxisRaw("Move Horizontal"), m_playerInputController.GetAxisRaw("Move Vertical"));
		m_playerController.SetDirectionalInput(directionalInput);

		Vector2 mouseAimInput = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
		m_playerController.SetGunnerAimInput(Camera.main.ScreenToWorldPoint(mouseAimInput) - transform.position);
		m_playerController.SetRunnerAimInput(Camera.main.ScreenToWorldPoint(mouseAimInput) - transform.position);

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

		if (Input.GetKeyDown(KeyCode.Space))
		{
			//m_playerController.OnMovementAbilityInputDown();
		}

		if (m_playerInputController.GetButtonDown("Movement Ability"))
		{
			m_playerController.OnMovementAbilityInputDown();
		}
	}
	#endregion

	#region Input Functions
	private void HandleRunnerInput(Player p_playerInputController)
	{
		Vector2 directionalInput = new Vector2(p_playerInputController.GetAxisRaw("Move Horizontal"), p_playerInputController.GetAxisRaw("Move Vertical"));
		m_playerController.SetDirectionalInput(directionalInput);

		Vector2 aimInput = new Vector2(p_playerInputController.GetAxisRaw("Runner Aim Horizontal"), p_playerInputController.GetAxisRaw("Runner Aim Vertical"));

		if (aimInput.normalized.magnitude != 0)
		{
			m_lastRunnerInput = aimInput;
			m_playerController.SetRunnerAimInput(aimInput);
		}
		else
		{
			m_playerController.SetRunnerAimInput(m_lastRunnerInput);
		}

		if (p_playerInputController.GetButtonDown("Jump"))
		{
			m_playerController.OnJumpInputDown();
		}
		if (p_playerInputController.GetButtonUp("Jump"))
		{
			m_playerController.OnJumpInputUp();
		}

		if (p_playerInputController.GetButtonDown("Reload"))
		{
			//m_playerController.OnReloadInputDown();
			m_runnerSwap = true;


            if(m_playerController.m_players[0].m_currentRole == PlayerController.PlayerRole.Runner)
            {
                m_humanSwap = true;
            }
            else
            {
                m_humanSwap = false;
            }
            m_swapPressed.Invoke(true);
        }

		if (p_playerInputController.GetButtonDown("Shoot"))
		{
			m_playerController.OnMovementAbilityInputDown();
		}
	}

	private void HandleGunnerInput(Player p_playerInputController)
	{
		Vector2 aimInput = new Vector2(p_playerInputController.GetAxisRaw("Aim Horizontal"), p_playerInputController.GetAxisRaw("Aim Vertical"));
		m_playerController.SetGunnerAimInput(aimInput);

		if (p_playerInputController.GetButton("Shoot"))
		{
			m_playerController.OnShootInputHold();
		}

		if (p_playerInputController.GetButtonDown("Reload"))
		{
			//m_playerController.OnReloadInputDown();
			m_gunnerSwap = true;

            if (m_playerController.m_players[0].m_currentRole == PlayerController.PlayerRole.Gunner)
            {
                m_humanSwap = true;
            }
            else
            {
                m_humanSwap = false;
            }

            m_swapPressed.Invoke(true);
        }
	}
	#endregion
}
