using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class OnPlayerSwap : UnityEvent { }

[System.Serializable]
public class OnPlayerHurt : UnityEvent { }

[System.Serializable]
public class OnPlayerJump : UnityEvent { }

[System.Serializable]
public class OnPlayerLand : UnityEvent { }

[System.Serializable]
public class OnPlayerRespawn : UnityEvent { }

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour, IPauseable
{
	public enum MovementControllState {MovementEnabled, MovementDisabled}
	public enum GravityState { GravityEnabled, GravityDisabled }
	public enum DamageState { Vulnerable, Invulnerable }
	public enum InputState { InputEnabled, InputDisabled }
	public enum SwappingState { SwappingEnabled, SwappingDisabled }
	public PlayerState m_states;
	public enum PlayerType { Robot, Alien }
	public enum PlayerRole { Runner, Gunner }
	public PlayerData[] m_players;
	private LayerMask m_gunnerDamageTargetMask;
	private LayerMask m_gunnerObstacleMask;
	private LayerMask m_runnerDamageTargetMask;
	private LayerMask m_runnerObstacleMask;

	#region Jump Properties
	[Header("Jump Properties")]
    public float m_maxJumpHeight = 4;
    public float m_minJumpHeight = 1;
    public float m_timeToJumpApex = .4f;

    float m_gravity;
    float m_maxJumpVelocity;
    float m_minJumpVelocity;
    [Space]
    #endregion

    #region Move Properties
    [Header("Run Properties")]
    public float m_accelerationTimeAirborne = .2f;
    public float m_accelerationTimeGrounded = .1f;
    public float m_moveSpeed = 6;

    private float m_velocityXSmoothing;
	private Vector2 m_velocitySmoothing;
	[HideInInspector]
	public Vector3 m_moveDirection;
    [Space]
    #endregion

    #region Aim Properties
    [Header("Aim Properties")]
    public Transform m_crosshair;
	public Transform m_shootPivotPoint;
    public float m_crosshairDst;
	[HideInInspector]
	public Vector3 m_gunnerAimDirection;
	[HideInInspector]
	public Vector3 m_runnerAimDirection;

    Vector3 m_lastPos;
    [Space]
    #endregion

    #region Wall Slide Properties
    [Header("Wall Slide Properties")]
    public float m_wallSlideSpeedMax = 3;
    public float m_wallStickTime = .25f;
    public Vector2 m_wallJumpClimb;
    public Vector2 m_wallJumpOff;
    public Vector2 m_wallLeap;

    float m_timeToWallUnstick;
    bool m_wallSliding;
    int m_wallDirX;
    [Space]
    #endregion

    #region Input Buffering Properties
    [Header("Input Buffering Properties")]
    public float m_graceTime;
    public float m_bufferTime;

    float m_graceTimer;
    float m_bufferTimer;
	private bool m_isLanded;
	[Space]
    #endregion

    #region Shoot Properties
    [Header("Shooting Properties")]
    private ShootController m_shootController;
	[Space]
	#endregion

	#region Movement Ability Properties
	[Header("Movement Ability Properties")]
	[SerializeField]
	private MovementAbilityComposition m_currentMovementAbilityComposition;
	public bool m_usingMovementAbility;
	public int m_movementAbilityAmmoCount;
	#endregion

	#region Events
	[Header("Events")]
	public OnPlayerSwap m_playerSwapped = new OnPlayerSwap();
	public OnPlayerHurt m_playerHurt = new OnPlayerHurt();
	public OnPlayerJump m_playerJumped = new OnPlayerJump();
	public OnPlayerLand m_playerLanded = new OnPlayerLand();
	public OnPlayerRespawn m_playerRespawned = new OnPlayerRespawn();
	#endregion

	public Transform m_spriteTarget;

	[HideInInspector]
    public Vector3 m_velocity;
	[HideInInspector]
    public Controller2D controller;
	[HideInInspector]
	public Vector2 m_directionalInput;
	[HideInInspector]
	public Vector2 m_gunnerAimInput;
	[HideInInspector]
	public Vector2 m_runnerAimInput;
	private Health_Player m_health;
	private PlayerInput m_input;
	private SpriteRenderer m_spriteRenderer;

	Vector3 m_velocityBeforePaused;

	[HideInInspector]
	public PlayerData[] m_playerDataAtRoomStart;

	void Start()
    {
        controller = GetComponent<Controller2D>();
		m_shootController = GetComponent<ShootController>();
		m_health = GetComponent<Health_Player>();
		m_input = GetComponent<PlayerInput>();
		m_spriteRenderer = GetComponentInChildren<SpriteRenderer>();

		CalculateJump();

		m_playerDataAtRoomStart = new PlayerData[m_players.Length];

		InitalizePlayer();

		PauseMenuController.instance.m_pauseables.Add(this);
	}

	void Update()
    {
        HandleWallSliding();
        InputBuffering();
        GunnerAim();
		UpdatePlayerStates();

		if (!controller.collisions.below)
		{
			m_isLanded = false;
		}

		m_moveDirection = m_velocity.normalized;

		if (Mathf.Sign(m_spriteTarget.transform.localPosition.x) == Mathf.Sign(controller.collisions.faceDir))
		{
			m_spriteTarget.transform.localPosition = new Vector2(Mathf.Sign(m_spriteTarget.transform.localPosition.x) > 0 ? m_spriteTarget.transform.localPosition.x * -controller.collisions.faceDir : m_spriteTarget.transform.localPosition.x * controller.collisions.faceDir, m_spriteTarget.transform.localPosition.y);
		}

		controller.Move(m_velocity * Time.deltaTime, m_directionalInput);

		CalculateGroundPhysics();
    }

	public void InitalizePlayer()
	{
		m_health.m_currentHealth = m_health.m_maxHealth;

		m_players.CopyTo(m_playerDataAtRoomStart, 0);

		UpdateLayers();
		UpdateInput();
		UpdatePickups();

		m_shootController.Reload();
		ReloadMovementAbility();

		ZeroPlayerVelocity();
	}

	public void Respawn(Vector3 p_respawnPosition)
	{
		transform.position = p_respawnPosition;
		m_playerRespawned.Invoke();
		m_playerDataAtRoomStart.CopyTo(m_players, 0);
		InitalizePlayer();	
	}

	public void ZeroPlayerVelocity()
	{
		m_velocityXSmoothing = 0;
		m_velocitySmoothing = Vector3.zero;
		m_velocity = Vector3.zero;
	}

	#region Input Code
	public void SetDirectionalInput(Vector2 p_input)
    {
        m_directionalInput = p_input;
    }

    public void SetGunnerAimInput(Vector2 p_input)
    {
        m_gunnerAimInput = p_input;
    }

	public void SetRunnerAimInput(Vector2 p_input)
	{
		m_runnerAimInput = p_input;
	}
	#endregion

	#region Aim Code
	private void GunnerAim()
    {
        float theta = Mathf.Atan2(m_gunnerAimInput.y, m_gunnerAimInput.x);

        float aimDegrees = theta * Mathf.Rad2Deg;

        Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * m_crosshairDst;

		m_gunnerAimDirection = m_crosshair.position - m_shootPivotPoint.position;

		if (m_gunnerAimInput.normalized.magnitude != 0)
        {
            m_crosshair.rotation = Quaternion.Euler(0, 0, aimDegrees);
            m_crosshair.position = m_shootPivotPoint.position + pCircle;
            m_lastPos = pCircle;
        }
        else
        {
            m_crosshair.position = m_shootPivotPoint.position + m_lastPos;
        }
    }
	#endregion

	#region Input Buffering Code
	void InputBuffering()
    {
        if (controller.collisions.below)
        {
            m_graceTimer = 0;

        }

        if (!controller.collisions.below)
        {
            m_graceTimer += Time.deltaTime;
        }

        if (m_bufferTimer > 0)
        {
            m_bufferTimer -= Time.deltaTime;
        }

        if (m_bufferTimer > 0 && controller.collisions.below)
        {
            m_bufferTimer = 0;

            if (controller.collisions.slidingDownMaxSlope)
            {
                if (m_directionalInput.x != -Mathf.Sign(controller.collisions.slopeNormal.x))
                { // not jumping against max slope
                    m_velocity.y = m_maxJumpVelocity * controller.collisions.slopeNormal.y;
                    m_velocity.x = m_maxJumpVelocity * controller.collisions.slopeNormal.x;
                }
            }
            else
            {
				JumpMaxVelocity();
            }
		}
		else if (controller.collisions.below && !m_isLanded)
		{
			m_playerLanded.Invoke();
			m_isLanded = true;
		}
    }
    #endregion

    #region Jump Code
    public void OnJumpInputDown()
    {
        m_bufferTimer = m_bufferTime;

        if (m_wallSliding)
        {
            if (m_wallDirX == m_directionalInput.x)
            {
                m_velocity.x = -m_wallDirX * m_wallJumpClimb.x;
                m_velocity.y = m_wallJumpClimb.y;
            }
            else if (m_directionalInput.x == 0)
            {
                m_velocity.x = -m_wallDirX * m_wallJumpOff.x;
                m_velocity.y = m_wallJumpOff.y;
            }
            else
            {
                m_velocity.x = -m_wallDirX * m_wallLeap.x;
                m_velocity.y = m_wallLeap.y;
            }
        }

        if (!controller.collisions.below && m_graceTimer <= m_graceTime && m_velocity.y <= 0)
        {
			JumpMaxVelocity();
            m_graceTimer = m_graceTime;
        }
    }

    public void OnJumpInputUp()
    {
        m_bufferTimer = 0;

        if (m_velocity.y > m_minJumpVelocity)
        {
			JumpMinVelocity();
        }
    }

	public void JumpMaxVelocityMultiplied(float p_jumpVelocityMultiplier)
	{
		if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
		{
			m_velocity.y = m_maxJumpVelocity * p_jumpVelocityMultiplier;
			m_playerJumped.Invoke();
		}
	}

	public void JumpMaxVelocity()
	{
		if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
		{
			m_velocity.y = m_maxJumpVelocity;
			m_playerJumped.Invoke();
		}
	}

	public void JumpMinVelocity()
	{
		if (m_states.m_movementControllState == MovementControllState.MovementEnabled)
		{
			m_velocity.y = m_minJumpVelocity;
		}
	}
    #endregion

    #region Wall Sliding Code
    void HandleWallSliding()
    {
        m_wallDirX = (controller.collisions.left) ? -1 : 1;
        m_wallSliding = false;

        if ((controller.collisions.left || controller.collisions.right) && m_directionalInput.x == m_wallDirX && !controller.collisions.below && m_velocity.y < 0)
        {
            m_wallSliding = true;

            if (m_velocity.y < -m_wallSlideSpeedMax)
            {
                m_velocity.y = -m_wallSlideSpeedMax;
            }

            if (m_timeToWallUnstick > 0)
            {
                m_velocityXSmoothing = 0;
                m_velocity.x = 0;

                if (m_directionalInput.x != m_wallDirX && m_directionalInput.x != 0)
                {
                    m_timeToWallUnstick -= Time.deltaTime;
                }
                else
                {
                    m_timeToWallUnstick = m_wallStickTime;
                }
            }
            else
            {
                m_timeToWallUnstick = m_wallStickTime;
            }
        }
    }
    #endregion

	#region Shoot Code
	public void OnShootInputHold()
    {
        m_shootController.Shoot(m_crosshair);
    }

    public void OnReloadInputDown()
    {
		if (m_states.m_swappingState == SwappingState.SwappingEnabled)
		{
			m_states.m_swappingState = SwappingState.SwappingDisabled;
			SwapPlayers();
			m_shootController.Reload();
			ReloadMovementAbility();
		}
	}
	#endregion

	#region Movement Ability Code
	public void OnMovementAbilityInputDown()
	{
		if (m_movementAbilityAmmoCount > 0)
		{
			if (!m_usingMovementAbility)
			{
				m_currentMovementAbilityComposition.UseAbility(this, m_runnerDamageTargetMask, m_runnerObstacleMask);

				m_movementAbilityAmmoCount--;
			}
		}
	}

	private void ReloadMovementAbility()
	{
		m_movementAbilityAmmoCount = m_currentMovementAbilityComposition.m_movementType.m_ammoCount;
	}

	[System.Serializable]
	public struct MovementAbilityComposition
	{
		public MovementType_Base m_movementType;
		public TrailType_Base m_trailType;
		public PlayerBuff_Base m_buffType;

		public void UseAbility(PlayerController p_player, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
		{
			m_movementType.UseAbility(p_player, m_trailType, m_buffType, p_damageTargetMask, p_obstacleMask);
		}
	}
	#endregion

	#region Physics Calculation Code
	private void CalculateJump()
	{
		m_gravity = -(2 * m_maxJumpHeight) / Mathf.Pow(m_timeToJumpApex, 2);
		m_maxJumpVelocity = Mathf.Abs(m_gravity) * m_timeToJumpApex;
		m_minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(m_gravity) * m_minJumpHeight);
	}

	void CalculateVelocity()
    {
		if (m_states.m_gravityControllState == GravityState.GravityEnabled)
		{
			float targetVelocityX = m_directionalInput.x * m_moveSpeed;
			m_velocity.x = Mathf.SmoothDamp(m_velocity.x, targetVelocityX, ref m_velocityXSmoothing, (controller.collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
			m_velocity.y += m_gravity * Time.deltaTime;
		}
		else if (m_states.m_gravityControllState == GravityState.GravityDisabled)
		{
			Vector2 targetVelocity = m_directionalInput * m_moveSpeed;
			m_velocity = Vector2.SmoothDamp(m_velocity, targetVelocity, ref m_velocitySmoothing, (controller.collisions.below) ? m_accelerationTimeGrounded : m_accelerationTimeAirborne);
		}
	}

	private void CalculateGroundPhysics()
	{
		if (controller.collisions.above || controller.collisions.below)
		{
			if (controller.collisions.slidingDownMaxSlope)
			{
				m_velocity.y += controller.collisions.slopeNormal.y * -m_gravity * Time.deltaTime;
			}
			else
			{
				m_velocity.y = 0;
			}
		}
	}
	#endregion

	#region Swaping Code
	private void SwapPlayers()
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			m_players[i].Swap();
		}

		m_playerSwapped.Invoke();
		UpdateLayers();
		UpdatePickups();
		UpdateInput();
	}

	private void UpdateInput()
	{
		if (m_players[0].m_currentRole == PlayerRole.Runner)
		{
			m_input.m_currentPlayerOrder = PlayerInput.PlayerOrder.ZeroRunnerOneGunner;
		}
		else if (m_players[0].m_currentRole == PlayerRole.Gunner)
		{
			m_input.m_currentPlayerOrder = PlayerInput.PlayerOrder.ZeroGunnerOneGunner;
		}
	}

	private void UpdateLayers()
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Gunner)
			{
				m_gunnerDamageTargetMask = m_players[i].m_damageTargetMask;
				m_gunnerObstacleMask = m_players[i].m_obstacleMask;

			}

			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_runnerDamageTargetMask = m_players[i].m_damageTargetMask;
				m_runnerObstacleMask = m_players[i].m_obstacleMask;
			}
		}

		SetLayersToComponents();
	}

	public void DisableSwapping()
	{
		m_states.m_swappingState = SwappingState.SwappingDisabled;
	}

	public void EnableSwapping()
	{
		m_states.m_swappingState = SwappingState.SwappingEnabled;
	}

	private void SetLayersToComponents()
	{
		m_shootController.m_damageTargetMask = m_gunnerDamageTargetMask;
		m_shootController.m_obstacleMask = m_runnerObstacleMask;
		controller.collisionMask = m_runnerObstacleMask;
	}

	[System.Serializable]
	public struct PlayerData
	{
		public PlayerType m_type;
		public PlayerRole m_currentRole;
		public LayerMask m_damageTargetMask;
		public LayerMask m_obstacleMask;
		public ShootController.WeaponComposition m_weaponComposition;
		public MovementAbilityComposition m_movementAbilityComposition;

		public void Swap()
		{
			if (m_currentRole == PlayerRole.Gunner)
			{
				m_currentRole = PlayerRole.Runner;
			}
			else
			{
				m_currentRole = PlayerRole.Gunner;
			}
		}
	}
	#endregion

	#region Pickups Code
	private void UpdatePickups()
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Gunner)
			{
				m_shootController.m_currentWeaponComposition = m_players[i].m_weaponComposition;
			}

			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_currentMovementAbilityComposition = m_players[i].m_movementAbilityComposition;
			}
		}

		m_shootController.Reload();
		ReloadMovementAbility();
	}

	public void SetShotPatternPickup(ShotPattern_Base p_newShotPattern)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Gunner)
			{
				m_players[i].m_weaponComposition.m_shotPattern = p_newShotPattern;
			}
		}

		UpdatePickups();
	}

	public void SetBulletTypePickup(Bullet_Base p_newBulletType)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Gunner)
			{
				m_players[i].m_weaponComposition.m_bulletType = p_newBulletType;
			}
		}

		UpdatePickups();
	}

	public void SetDamageTypePickup(DamageType_Base p_newDamageType)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Gunner)
			{
				m_players[i].m_weaponComposition.m_damageType = p_newDamageType;
			}
		}

		UpdatePickups();
	}

	public void SetMovementTypePickup(MovementType_Base p_newMovementType)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_players[i].m_movementAbilityComposition.m_movementType = p_newMovementType;
			}
		}

		UpdatePickups();
	}

	public void SetTrailTypePickup(TrailType_Base p_newTrailType)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_players[i].m_movementAbilityComposition.m_trailType = p_newTrailType;
			}
		}

		UpdatePickups();
	}

	public void SetBuffPickup(PlayerBuff_Base p_newBuffType)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_players[i].m_movementAbilityComposition.m_buffType = p_newBuffType;
			}
		}

		UpdatePickups();
	}
	#endregion

	#region Player State Code
	[System.Serializable]
	public struct PlayerState
	{
		public MovementControllState m_movementControllState;
		public GravityState m_gravityControllState;
		public DamageState m_damageState;
		public InputState m_inputState;
		public SwappingState m_swappingState;
	}

	private void UpdatePlayerStates()
	{
		switch (m_states.m_movementControllState)
		{
			case MovementControllState.MovementEnabled:

				CalculateVelocity();

				break;

			case MovementControllState.MovementDisabled:

				//Nothing

				break;
		}

		switch (m_states.m_damageState)
		{
			case DamageState.Vulnerable:

				m_health.m_canTakeDamage = true;

				break;

			case DamageState.Invulnerable:

				m_health.m_canTakeDamage = false;

				break;
		}

		switch (m_states.m_inputState)
		{
			case InputState.InputEnabled:

				//Nothing

				break;

			case InputState.InputDisabled:



				break;
		}

		if (m_usingMovementAbility)
		{
			m_states.m_swappingState = SwappingState.SwappingDisabled;
		}
		else
		{
			m_states.m_swappingState = SwappingState.SwappingEnabled;
		}
	}
	#endregion

	public void SetPauseState(bool p_isPaused)
	{
		if (p_isPaused)
		{
			m_velocityBeforePaused = m_velocity;

			if (m_usingMovementAbility)
			{
				m_usingMovementAbility = false;
				m_states.m_gravityControllState = GravityState.GravityDisabled;
			}

			m_velocity = Vector3.zero;
			m_states.m_inputState = InputState.InputDisabled;
			m_states.m_movementControllState = MovementControllState.MovementDisabled;
		}
		else
		{
			m_states.m_inputState = InputState.InputEnabled;
			m_states.m_movementControllState = MovementControllState.MovementEnabled;
			m_states.m_gravityControllState = GravityState.GravityEnabled;

			m_velocity = m_velocityBeforePaused;
		}
	}
}
