using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController_Base : MonoBehaviour
{
	[Header("Global Camera Properites")]
	public Controller2D m_target;

	private PlayerController m_player;
	private Camera m_camera;
	private Vector3 m_focusPoint;

	[Header("Runner Camera Properties")]
	public float m_verticalOffset;
	public float m_lookAheadDstX;
	public float m_lookSmoothTimeX;
	public float m_verticalSmoothTime;
	public Vector2 m_runnerFocusAreaSize;

	private FocusArea m_runnerFocusArea;
	private float m_currentLookAheadX;
	private float m_targetLookAheadX;
	private float m_lookAheadDirX;
	private float m_smoothLookVelocityX;
	private float m_smoothVelocityY;
	private bool m_lookAheadStopped;

	[Header("Gunner Camera Properties")]
	public float m_gunnerPanDistance;
	public float m_gunnerPanSmoothTime;

	private Vector3 m_lookSmoothVelocity;

	[Header("Camera Level Bounds Properties")]
	public bool m_useLevelBounds = false;
	public Bounds m_cameraBoundsArea;
	public TilemapCollider2D m_firstTileMapCollider;

	void Start()
	{
		m_runnerFocusArea = new FocusArea(m_target.col.bounds, m_runnerFocusAreaSize);

		m_player = m_target.GetComponent<PlayerController>();

		m_camera = GetComponent<Camera>();

		if (m_useLevelBounds)
		{
			CalculateNewCameraBounds(m_firstTileMapCollider);
		}
	}

	void LateUpdate()
	{
		m_runnerFocusArea.Update(m_target.col.bounds);

		CalculateRunnerCamera();

		CalculateGunnerCamera();

		if (m_useLevelBounds)
		{
			if (!m_cameraBoundsArea.Contains(transform.position))
			{
				transform.position = m_cameraBoundsArea.ClosestPoint(transform.position);
			}
		}
	}

	private void CalculateGunnerCamera()
	{
		float theta = Mathf.Atan2(m_player.m_gunnerAimInput.y, m_player.m_gunnerAimInput.x);
		Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * m_gunnerPanDistance;
		Vector3 targetPos = m_focusPoint + (pCircle + Vector3.forward * -10);

		Debug.DrawLine(m_focusPoint, targetPos);

		transform.position = Vector3.SmoothDamp(transform.position, targetPos, ref m_lookSmoothVelocity, m_gunnerPanSmoothTime);
	}

	private void CalculateRunnerCamera()
	{
		Vector2 focusPosition = m_runnerFocusArea.centre + Vector2.up * m_verticalOffset;

		if (m_runnerFocusArea.velocity.x != 0)
		{
			m_lookAheadDirX = Mathf.Sign(m_runnerFocusArea.velocity.x);

			if (Mathf.Sign(m_target.playerInput.x) == Mathf.Sign(m_runnerFocusArea.velocity.x) && m_target.playerInput.x != 0)
			{
				m_lookAheadStopped = false;
				m_targetLookAheadX = m_lookAheadDirX * m_lookAheadDstX;
			}
			else
			{
				if (!m_lookAheadStopped)
				{
					m_lookAheadStopped = true;
					m_targetLookAheadX = m_currentLookAheadX + (m_lookAheadDirX * m_lookAheadDstX - m_currentLookAheadX) / 4f;
				}
			}
		}

		m_currentLookAheadX = Mathf.SmoothDamp(m_currentLookAheadX, m_targetLookAheadX, ref m_smoothLookVelocityX, m_lookSmoothTimeX);
		focusPosition.y = Mathf.SmoothDamp(m_focusPoint.y, focusPosition.y, ref m_smoothVelocityY, m_verticalSmoothTime);
		focusPosition += Vector2.right * m_currentLookAheadX;
		m_focusPoint = (Vector3)focusPosition + Vector3.forward * -10;
	}

	private void CalculateNewCameraBounds(TilemapCollider2D p_tilemapCollider)
	{
		Bounds newBounds = new Bounds();

		float camHeight = 2f * m_camera.orthographicSize;
		float camWidth = camHeight * m_camera.aspect;

		float boundsHeight = ((p_tilemapCollider.bounds.extents.y * 2) - camHeight) / 2;
		float boundsWidth = ((p_tilemapCollider.bounds.extents.x * 2) - camWidth) / 2;

		newBounds.extents = new Vector3(boundsWidth, boundsHeight, 0);

		newBounds.center = p_tilemapCollider.bounds.center;

		if (newBounds.center.z != transform.position.z)
		{
			newBounds.center = new Vector3(newBounds.center.x, newBounds.center.y, transform.position.z);
		}

		m_cameraBoundsArea = newBounds;
	}

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, .5f);
		Gizmos.DrawCube(m_runnerFocusArea.centre, m_runnerFocusAreaSize);

		DebugExtension.DrawBounds(m_cameraBoundsArea, Color.blue);
	}

	#region Focus Struct
	struct FocusArea
	{
		public Vector2 centre;
		public Vector2 velocity;
		public float left, right;
		public float top, bottom;


		public FocusArea(Bounds targetBounds, Vector2 size)
		{
			left = targetBounds.center.x - size.x / 2;
			right = targetBounds.center.x + size.x / 2;
			bottom = targetBounds.min.y;
			top = targetBounds.min.y + size.y;

			velocity = Vector2.zero;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
		}

		public void Update(Bounds targetBounds)
		{
			float shiftX = 0;
			if (targetBounds.min.x < left)
			{
				shiftX = targetBounds.min.x - left;
			}
			else if (targetBounds.max.x > right)
			{
				shiftX = targetBounds.max.x - right;
			}
			left += shiftX;
			right += shiftX;

			float shiftY = 0;
			if (targetBounds.min.y < bottom)
			{
				shiftY = targetBounds.min.y - bottom;
			}
			else if (targetBounds.max.y > top)
			{
				shiftY = targetBounds.max.y - top;
			}
			top += shiftY;
			bottom += shiftY;
			centre = new Vector2((left + right) / 2, (top + bottom) / 2);
			velocity = new Vector2(shiftX, shiftY);
		}
	}
	#endregion
}
