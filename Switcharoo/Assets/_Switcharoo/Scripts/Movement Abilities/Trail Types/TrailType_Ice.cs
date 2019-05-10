using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Ice")]
public class TrailType_Ice : TrailType_Base
{
	[Header("Ice Trail Properites")]
	public int m_amountOfDropsPerUse;
	public TrailObject_Ice m_dropObject;

	[Header("Ice Explosion Properites")]
	public float m_iceExplosionRadius;

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

	public void IceBlast(Vector3 p_blastOrigin, LayerMask p_damageTargetMask)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_blastOrigin, m_iceExplosionRadius, p_damageTargetMask);

		DebugExtension.DebugCircle(p_blastOrigin, Vector3.forward, Color.cyan, m_iceExplosionRadius, 0.1f);

		foreach (Collider2D collider in colliders)
		{
			collider.GetComponent<Health>().SetIceState();
		}
	}

	private void DropIce(PlayerController p_playerRefrence)
	{
	 	GameObject newDropObject = ObjectPooler.instance.NewObject(m_dropObject.gameObject, p_playerRefrence.transform, true);

		newDropObject.GetComponent<TrailObject_Ice>().m_trailType = this;
		newDropObject.GetComponent<TrailObject_Ice>().m_damageTargetMask = p_playerRefrence.m_gunnerDamageTargetMask;
	}
}
