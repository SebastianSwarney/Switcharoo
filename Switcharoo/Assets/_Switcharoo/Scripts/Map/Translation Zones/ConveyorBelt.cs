using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConveyorBelt : MonoBehaviour, IActivatable, IPauseable
{
    /*
	public enum MoveDirection { Left, Right, Up, Down }
	public MoveDirection m_moveDirection;
	*/

    public float m_moveSpeed;

    [Header("Active Properties")]
    public bool m_startActive;
    bool m_isActive;
    bool m_isPaused;


    private void Start()
    {
        ObjectPooler.instance.AddObjectToPooler(gameObject);
        m_isActive = m_startActive;
    }
    private void OnCollisionStay2D(Collision2D collision)
    {

        if (m_isPaused) return;
        if (m_isActive)
        {


            PlayerController player = collision.gameObject.GetComponent<PlayerController>();

            player.m_velocity += transform.right * m_moveSpeed;

        }
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

    #region IActivatable
    public void ActiveState(bool p_active)
    {
        m_isActive = p_active;
    }

    public void ResetMe()
    {
        m_isActive = !m_startActive;
    }


    #endregion

    public void SetPauseState(bool p_isPaused)
    {
        m_isPaused = p_isPaused;
    }
}
