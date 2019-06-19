using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour
{
	/*
	public enum MoveDirection { Left, Right, Up, Down }
	public MoveDirection m_moveDirection;
	*/

	public float m_moveSpeed;

	private void OnCollisionStay2D(Collision2D collision)
	{
		PlayerController player = collision.gameObject.GetComponent<PlayerController>();

		player.m_velocity += transform.right * m_moveSpeed;

		/*
		switch (m_moveDirection)
		{
			case MoveDirection.Left:

				player.m_velocity += Vector3.left * m_moveSpeed;

				break;
			case MoveDirection.Right:

				player.m_velocity += Vector3.right * m_moveSpeed;

				break;
			case MoveDirection.Up:

				player.m_velocity += Vector3.up * m_moveSpeed;

				break;
			case MoveDirection.Down:

				player.m_velocity += Vector3.down * m_moveSpeed;

				break;
		}
		*/
	}
}
