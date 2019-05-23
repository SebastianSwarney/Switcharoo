using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionHazard_Rotate : CollisionHazard_Base
{
	[Header("Rotate Hazard Properties")]
	[Range(0, 180)]
	public float m_rotateDegrees;
	public float m_rotateSpeed;
	public Transform m_rotateTarget;

	private float m_rotateTimer = 0;

	private void Update()
	{
		Rotate();
	}

	private void Rotate()
	{
		m_rotateTimer += Time.deltaTime;
		float angle = Mathf.Sin(m_rotateTimer * m_rotateSpeed) * m_rotateDegrees;
		m_rotateTarget.localRotation = Quaternion.AngleAxis(angle, transform.forward);
	}

	private void OnDrawGizmos()
	{
		//float lineLength = GetComponentInChildren<BoxCollider2D>().size.x;

		Vector3 lineEnd1 = Quaternion.Euler(0, 0, m_rotateDegrees) * -transform.right * 10;
		Debug.DrawLine(transform.position, transform.position - lineEnd1, Color.red);

		Vector3 lineEnd2 = Quaternion.Euler(0, 0, -m_rotateDegrees) * -transform.right * 10;
		Debug.DrawLine(transform.position, transform.position - lineEnd2, Color.red);
	}
}
