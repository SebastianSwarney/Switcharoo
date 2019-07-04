using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Trail Types/Fire")]
public class TrailType_Fire : TrailType_Base
{
	[Header("Fire Trail Properites")]
	public TrailObject_Fire m_dropObject;
	public float m_orbShootForce;
	public float m_orbSpawnAngleMax;


    public override void UseTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		p_playerRefrence.StartCoroutine(FireTrail(p_playerRefrence, p_movementType, p_damageTargetMask, p_obstacleMask));
	}

	private IEnumerator FireTrail(PlayerController p_playerRefrence, MovementType_Base p_movementType, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		float t = 0;

		while (t < p_movementType.m_movementTime)
		{
			Vector3 moveDir = p_playerRefrence.m_moveDirection;
			t += Time.deltaTime;
			DropFire(p_playerRefrence.transform, moveDir, p_damageTargetMask, p_playerRefrence);

			if (!p_playerRefrence.m_usingMovementAbility)
			{
				t = p_movementType.m_movementTime;
			}

			yield return null;
		}
	}

	private void DropFire(Transform p_spawnPoint, Vector3 p_moveDirection, LayerMask p_damageTargetMask, PlayerController p_playerRefrence)
	{
		GameObject newDropObject = ObjectPooler.instance.NewObject(m_dropObject.gameObject, p_spawnPoint, true);
		float randomAngle = Random.Range(-m_orbSpawnAngleMax / 2, m_orbSpawnAngleMax / 2);
		Vector3 forceDir = Quaternion.Euler(0, 0, randomAngle) * -p_moveDirection;
		newDropObject.GetComponent<Rigidbody2D>().AddForce(forceDir * m_orbShootForce, ForceMode2D.Impulse);
		newDropObject.GetComponent<TrailObject_Fire>().m_damageTargetMask = p_damageTargetMask;
		newDropObject.GetComponent<TrailObject_Fire>().m_type = p_playerRefrence.m_currentRunnerType;
	}
}
