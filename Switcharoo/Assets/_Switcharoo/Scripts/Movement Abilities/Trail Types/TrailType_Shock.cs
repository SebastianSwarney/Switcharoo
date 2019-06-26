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

	public GameObject m_lightingEffect;
	public float m_effectsPerUnit;
	public float m_effectSpacing;



    public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(ShockTrail(p_playerRefrence, p_movementType, p_damageTargetMask, p_obstacleMask));
	}

	private IEnumerator ShockTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float dropInterval = p_movementType.m_movementTime / p_movementType.m_amountOfTrailsToSpawn;
		int amountOfDrops = 0;

		while (amountOfDrops < p_movementType.m_amountOfTrailsToSpawn)
		{
			DropShock(p_playerRefrence.transform, p_damageTargetMask);
			amountOfDrops++;

			yield return new WaitForSeconds(dropInterval);

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				amountOfDrops = (int)p_movementType.m_amountOfTrailsToSpawn;
			}
		}
	}

	private void DropShock(Transform p_spawnPoint, LayerMask p_damageTargetMask)
	{
		GameObject newDropObject = ObjectPooler.instance.NewObject(m_dropObject.gameObject, p_spawnPoint, true);
		newDropObject.GetComponent<TrailObject_Shock>().m_trailType = this;
		newDropObject.GetComponent<TrailObject_Shock>().m_damageTargetMask = p_damageTargetMask;
	}

	public void ShockBlast(Vector3 p_shockOrigin, LayerMask p_damageTargetMask)
	{
		List<Collider2D> shockedObjects = new List<Collider2D>();
		Collider2D[] initialCast = Physics2D.OverlapCircleAll(p_shockOrigin, m_shockRadius, p_damageTargetMask);
		shockedObjects.AddRange(initialCast);

		foreach (Collider2D collider in initialCast)
		{
			SpawnLightingEffects(p_shockOrigin, collider.transform.position);
		}

		DebugExtension.DebugCircle(p_shockOrigin, Vector3.forward, Color.blue, m_shockRadius, 0.1f);

		for (int i = 0; i < shockedObjects.Count && i < m_shockChainAmount; i++)
		{
			Collider2D[] childCast = Physics2D.OverlapCircleAll(shockedObjects[i].transform.position, m_shockRadius, p_damageTargetMask);

			foreach (Collider2D collider in childCast)
			{
				if (!shockedObjects.Contains(collider))
				{
					shockedObjects.Add(collider);
				}
			}

			DebugExtension.DebugCircle(shockedObjects[i].transform.position, Vector3.forward, Color.blue, m_shockRadius, 0.1f);

			if (i < shockedObjects.Count - 1)
			{
				SpawnLightingEffects(shockedObjects[i].transform.position, shockedObjects[i + 1].transform.position);
			}

		}

		foreach (Collider2D collider in shockedObjects)
		{
			collider.GetComponent<Health>().TakeDamage(m_shockDamage);
		}
	}

	public void SpawnLightingEffects(Vector3 p_startPos, Vector3 p_endPos)
	{
		Vector2 reletivePos = p_startPos - p_endPos;

		float theta = Mathf.Atan2(reletivePos.y, reletivePos.x);

		float thetaDegrees = theta * Mathf.Rad2Deg;

		int dst = Mathf.RoundToInt(Vector3.Distance(p_startPos, p_endPos));

		int effectsAmount = Mathf.RoundToInt(dst * m_effectsPerUnit);

		Vector3 pCircle = new Vector3(Mathf.Cos(theta), Mathf.Sin(theta), 0);

		for (int i = 0; i < effectsAmount; i++)
		{
			float spacing = i * (m_effectsPerUnit + m_effectSpacing);

			Vector3 spawnPos = p_endPos + pCircle * spacing;
			ObjectPooler.instance.NewObject(m_lightingEffect, spawnPos, Quaternion.Euler(0, 180, -thetaDegrees));
		}
	}
}
