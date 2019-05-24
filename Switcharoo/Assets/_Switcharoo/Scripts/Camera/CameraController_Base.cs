using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class CameraController_Base : MonoBehaviour
{
	public Controller2D m_target;
	public float m_verticalOffset;
	public float m_lookAheadDstX;
	public float m_lookSmoothTimeX;
	public float m_verticalSmoothTime;
	public Vector2 m_focusAreaSize;
	public Bounds m_cameraBoundsArea;

	public TilemapCollider2D m_firstTileMapCollider;

	[Space]

	public float m_cameraTransitionTime;
	public AnimationCurve m_transitionCurve;

	public Camera m_camera;

	private PlayerController m_player;

	FocusArea m_focusArea;

	private float m_currentLookAheadX;
	private float m_targetLookAheadX;
	private float m_lookAheadDirX;
	private float m_smoothLookVelocityX;
	private float m_smoothVelocityY;

	private bool m_lookAheadStopped;

	void Start()
	{
		m_focusArea = new FocusArea(m_target.col.bounds, m_focusAreaSize);
		m_camera = GetComponent<Camera>();

		m_player = m_target.GetComponent<PlayerController>();

		CalculateNewCameraBounds(m_firstTileMapCollider);
	}

	void LateUpdate()
	{
		m_focusArea.Update(m_target.col.bounds);

		Vector2 focusPosition = m_focusArea.centre + Vector2.up * m_verticalOffset;

		if (m_focusArea.velocity.x != 0)
		{
			m_lookAheadDirX = Mathf.Sign(m_focusArea.velocity.x);
			if (Mathf.Sign(m_target.playerInput.x) == Mathf.Sign(m_focusArea.velocity.x) && m_target.playerInput.x != 0)
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

		focusPosition.y = Mathf.SmoothDamp(transform.position.y, focusPosition.y, ref m_smoothVelocityY, m_verticalSmoothTime);
		focusPosition += Vector2.right * m_currentLookAheadX;
		transform.position = (Vector3)focusPosition + Vector3.forward * -10;

		if (!m_cameraBoundsArea.Contains(transform.position))
		{
			transform.position = m_cameraBoundsArea.ClosestPoint(transform.position);
		}

		Aim();
	}

	private void Aim()
	{
		float theta = Mathf.Atan2(m_player.m_gunnerAimInput.y, m_player.m_gunnerAimInput.x);

		Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0) * 1;

		m_camera.transform.position = transform.position + pCircle;

		//m_crosshair.position = transform.position + pCircle;
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

	#region Camera Transition Code
	IEnumerator TransitionCameraToNewLevel(float p_newCameraSize, TilemapCollider2D p_newTilemapCollider)
	{
		float t = 0;

		float startSize = m_camera.orthographicSize;

		Vector3 startPosition = transform.position;

		Vector3 targetPosition = p_newTilemapCollider.bounds.center;

		while (t < m_cameraTransitionTime)
		{
			t += Time.deltaTime;

			float progress = m_transitionCurve.Evaluate(t / m_cameraTransitionTime);

			transform.position = Vector3.Lerp(startPosition, targetPosition, progress);

			m_camera.orthographicSize = Mathf.Lerp(startSize, p_newCameraSize, progress);

			yield return null;
		}

		CalculateNewCameraBounds(p_newTilemapCollider);
	}
	#endregion

	void OnDrawGizmos()
	{
		Gizmos.color = new Color(1, 0, 0, .5f);
		Gizmos.DrawCube(m_focusArea.centre, m_focusAreaSize);

		DebugExtension.DrawBounds(m_cameraBoundsArea, Color.red);
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
