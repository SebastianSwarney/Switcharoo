using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletContactVisual_Base : MonoBehaviour
{
	private Animator m_animator;

	private void Start()
	{
		m_animator = GetComponent<Animator>();
	}

	private void Update()
	{
		if (m_animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !m_animator.IsInTransition(0))
		{
			ObjectPooler.instance.ReturnToPool(gameObject);
		}
	}

}
