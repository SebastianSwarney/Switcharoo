using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_ConstMove : Bullet_Base
{
    private void Update()
    {
		Move();
    }

	private void Move()
	{
		transform.Translate(transform.right * m_moveSpeed * Time.deltaTime, Space.World);
	}
}
