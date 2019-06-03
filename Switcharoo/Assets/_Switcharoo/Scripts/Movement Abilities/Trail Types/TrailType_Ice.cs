using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Ice")]
public class TrailType_Ice : TrailType_Base
{
	[Header("Ice Trail Properites")]
	public TrailObject_Ice m_dropObject;

	[Header("Ice Explosion Properites")]
	public float m_iceExplosionRadius;

	public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(IceTrail(p_playerRefrence, p_movementType, p_damageTargetMask, p_obstacleMask));
	}

	private IEnumerator IceTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float dropInterval = p_movementType.m_movementTime / p_movementType.m_amountOfTrailsToSpawn;
		int amountOfDrops = 0;

		while (amountOfDrops < p_movementType.m_amountOfTrailsToSpawn)
		{
			DropIce(p_playerRefrence.transform, p_damageTargetMask);
			amountOfDrops++;

			yield return new WaitForSeconds(dropInterval);
		}
	}

	public void IceBlast(Vector3 p_blastOrigin, LayerMask p_damageTargetMask)
	{
		DebugExtension.DebugCircle(p_blastOrigin, Vector3.forward, Color.cyan, m_iceExplosionRadius, 0.1f);

		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_blastOrigin, m_iceExplosionRadius, p_damageTargetMask);
		foreach (Collider2D collider in colliders)
		{
			collider.GetComponent<Health>().SetIceState();
		}
	}

	private void DropIce(Transform p_spawnPoint, LayerMask p_damageTargetMask)
	{
	 	GameObject newDropObject = ObjectPooler.instance.NewObject(m_dropObject.gameObject, p_spawnPoint, true);
		newDropObject.GetComponent<TrailObject_Ice>().m_trailType = this;
		newDropObject.GetComponent<TrailObject_Ice>().m_damageTargetMask = p_damageTargetMask;
	}
}
