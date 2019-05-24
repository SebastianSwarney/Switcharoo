using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour
{
	public enum MovementControllState {MovementEnabled, MovementDisabled}

	public enum GravityState { GravityEnabled, GravityDisabled }

	public enum DamageState { Vulnerable, Invulnerable }

	public PlayerState m_states;

	public enum PlayerType { Type0, Type1 }

	public enum PlayerRole { Runner, Gunner }

	public PlayerData[] m_players;

	public LayerMask m_gunnerDamageTargetMask;
	public LayerMask m_runnerObstacleMask;

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
    public float m_crosshairDst;
	[HideInInspector]
	public Vector3 m_aimDirection;

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
    [Space]
    #endregion

    #region Dash Properties
    [Header("Dash Properties")]
    public float m_dashDistance;
    public float m_dashTime;
    public float m_groundDashCooldown;
    public AnimationCurve m_dashCurve;

    //bool m_canDash = true;
    bool m_dashing;

    float m_dashSpeed;
    float m_dashingTime;
    float m_groundDashTimer;
    bool m_canDashGround;
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
	#endregion

	[HideInInspector]
    public Vector3 m_velocity;

	[HideInInspector]
    public Controller2D controller;

    private Vector2 m_directionalInput;

	public Vector2 m_gunnerAimInput;

	private Vector2 m_runnerAimInput;

	private Health_Player m_health;

	private PlayerInput m_input;

    void Start()
    {
        controller = GetComponent<Controller2D>();
		m_shootController = GetComponent<ShootController>();
		m_health = GetComponent<Health_Player>();
		m_input = GetComponent<PlayerInput>();

		CalculateJump();
		UpdatePickups();
		UpdateLayers();
		UpdateInput();
		m_shootController.Reload();
	}

	void Update()
    {
        HandleWallSliding();
        InputBuffering();
        Aim();
		UpdatePlayerStates();

		controller.Move(m_velocity * Time.deltaTime, m_directionalInput);

		CalculateGroundPhysics();
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
	void Aim()
    {
        float theta = Mathf.Atan2(m_gunnerAimInput.y, m_gunnerAimInput.x);

        float aimDegrees = theta * Mathf.Rad2Deg;

        Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * m_crosshairDst;

		m_aimDirection = m_crosshair.position - transform.position;

		if (m_gunnerAimInput.normalized.magnitude != 0)
        {
            m_crosshair.rotation = Quaternion.Euler(0, 0, aimDegrees);
            m_crosshair.position = transform.position + pCircle;
            m_lastPos = pCircle;
        }
        else
        {
            m_crosshair.position = transform.position + m_lastPos;
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
                m_velocity.y = m_maxJumpVelocity;
            }
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
            m_velocity.y = m_maxJumpVelocity;
            m_graceTimer = m_graceTime;
        }
    }

    public void OnJumpInputUp()
    {
        m_bufferTimer = 0;

        if (m_velocity.y > m_minJumpVelocity)
        {
            m_velocity.y = m_minJumpVelocity;
        }
    }

	public void JumpMaxVelocityMultiplied(float p_jumpVelocityMultiplier)
	{
		m_velocity.y = m_maxJumpVelocity * p_jumpVelocityMultiplier;
	}

	public void JumpMaxVelocity()
	{
		m_velocity.y = m_maxJumpVelocity;
	}

	public void JumpMinVelocity()
	{
		m_velocity.y = m_minJumpVelocity;
	}
    #endregion

    #region Wall Sliding Code
    void HandleWallSliding()
    {
        m_wallDirX = (controller.collisions.left) ? -1 : 1;
        m_wallSliding = false;
        if ((controller.collisions.left || controller.collisions.right) && !controller.collisions.below && m_velocity.y < 0)
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

    #region Dash Code
    public void OnDashInputDown()
    {
        if (!m_dashing)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        m_dashing = true;

        m_velocity = Vector3.zero;

        Vector3 initialPosition = transform.position;

        float dashTargetX = (m_directionalInput.x > 0) ? transform.position.x + m_dashDistance : transform.position.x - m_dashDistance; //possibly change this as the velocity x smoothing variable may have been the problem

        Vector3 dashTarget = new Vector3(dashTargetX, transform.position.y, transform.position.z);

        float t = 0;

        while (t < m_dashTime)
        {
            t += Time.deltaTime;

            float progress = m_dashCurve.Evaluate(t / m_dashTime);

            Vector3 targetPosition = Vector3.Lerp(initialPosition, dashTarget, progress);

            PhysicsSeekTo(targetPosition);

            yield return null;
        }

        m_velocity = Vector3.zero;

        m_velocityXSmoothing = 0; //figure out how to do this somewhere else

        m_dashing = false;
    }

	private void PhysicsSeekTo(Vector3 p_targetPosition)
	{
		Vector3 deltaPosition = p_targetPosition - transform.position;
		m_velocity = deltaPosition / Time.deltaTime;
	}
	#endregion

	#region Shoot Code
	public void OnShootInputHold()
    {
        m_shootController.Shoot(m_crosshair);
    }

    public void OnReloadInputDown()
    {
		SwapPlayers();

		m_shootController.Reload();
	}
	#endregion

	#region Movement Ability Code
	public void OnMovementAbilityInputDown()
	{
		m_currentMovementAbilityComposition.UseAbility(this);
	}

	[System.Serializable]
	public struct MovementAbilityComposition
	{
		public MovementType_Base m_movementType;
		public TrailType_Base m_trailType;

		public void UseAbility(PlayerController p_player)
		{
			m_movementType.UseAbility(p_player, m_trailType);
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
			UpdateLayers();
			UpdatePickups();
		}

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

				m_shootController.m_damageTargetMask = m_players[i].m_damageTargetMask;
				m_shootController.m_obstacleMask = m_players[i].m_obstacleMask;
			}

			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_runnerObstacleMask = m_players[i].m_obstacleMask;
			}
		}
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

	/*
	public void SetMovementPickup(MovementAbility_Base p_newMovementAbility)
	{
		for (int i = 0; i < m_players.Length; i++)
		{
			if (m_players[i].m_currentRole == PlayerRole.Runner)
			{
				m_players[i].m_movementAbility = p_newMovementAbility;
			}
		}

		UpdatePickups();
	}
	*/
	#endregion

	#region Player State Code
	[System.Serializable]
	public struct PlayerState
	{
		public MovementControllState m_movementControllState;
		public GravityState m_gravityControllState;
		public DamageState m_damageState;
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
	}
	#endregion
}
