using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Shock")]
public class TrailType_Shock : TrailType_Base
{
	[Header("Shock Trail Properites")]
	public TrailObject_Shock m_dropObject;

	[Header("Shock Blast Properties")]
	public float m_shockRadius;
	public int m_shockChainAmount;
	public float m_shockDamage;

	public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		p_playerRefrence.StartCoroutine(ShockTrail(p_playerRefrence, p_movementType));
	}

	private IEnumerator ShockTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType)
	{
		float dropInterval = p_movementType.m_movementTime / p_movementType.m_amountOfTrailsToSpawn;

		int amountOfDrops = 0;

		while (amountOfDrops < p_movementType.m_amountOfTrailsToSpawn)
		{
			DropShock(p_playerRefrence);

			amountOfDrops++;

			yield return new WaitForSeconds(dropInterval);
		}
	}

	private void DropShock(PlayerController p_playerRefrence)
	{
		GameObject newDropObject = ObjectPooler.instance.NewObject(m_dropObject.gameObject, p_playerRefrence.transform, true);

		newDropObject.GetComponent<TrailObject_Shock>().m_trailType = this;
		newDropObject.GetComponent<TrailObject_Shock>().m_damageTargetMask = p_playerRefrence.m_gunnerDamageTargetMask;
	}

	public void ShockBlast(Vector3 p_shockOrigin, LayerMask p_damageTargetMask)
	{
		List<Collider2D> shockedObjects = new List<Collider2D>();

		Collider2D[] initialCast = Physics2D.OverlapCircleAll(p_shockOrigin, m_shockRadius, p_damageTargetMask);

		DebugExtension.DebugCircle(p_shockOrigin, Vector3.forward, Color.blue, m_shockRadius, 0.1f);

		shockedObjects.AddRange(initialCast);

		for (int i = 0; i < shockedObjects.Count && i < m_shockChainAmount; i++)
		{
			Collider2D[] childCast = Physics2D.OverlapCircleAll(shockedObjects[i].transform.position, m_shockRadius, p_damageTargetMask);

			DebugExtension.DebugCircle(shockedObjects[i].transform.position, Vector3.forward, Color.blue, m_shockRadius, 0.1f);

			foreach (Collider2D collider in childCast)
			{
				if (!shockedObjects.Contains(collider))
				{
					shockedObjects.Add(collider);
				}
			}
		}

		foreach (Collider2D collider in shockedObjects)
		{
			collider.GetComponent<Health>().TakeDamage(m_shockDamage);
		}
	}
}
