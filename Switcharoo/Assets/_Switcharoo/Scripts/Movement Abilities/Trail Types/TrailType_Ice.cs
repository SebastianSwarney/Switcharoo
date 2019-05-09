using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Ice")]
public class TrailType_Ice : TrailType_Base
{
	[Header("Ice Properites")]
	public int m_amountOfDropsPerUse;
	public GameObject m_dropObject;

	public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		p_playerRefrence.StartCoroutine(IceTrail(p_playerRefrence, p_movementType));
	}

	private IEnumerator IceTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		float dropInterval = p_movementType.m_movementTime / m_amountOfDropsPerUse;

		int amountOfDrops = 0;

		while (amountOfDrops < m_amountOfDropsPerUse)
		{
			DropIce(p_playerRefrence);

			amountOfDrops++;

			yield return new WaitForSeconds(dropInterval);
		}
	}

	private void DropIce(PlayerController p_playerRefrence)
	{
	 	ObjectPooler.instance.NewObject(m_dropObject, p_playerRefrence.transform, true);
	}
}
