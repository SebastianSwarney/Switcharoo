using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Buff Types/Reflect")]
public class PlayerBuff_Reflect : PlayerBuff_Base
{
	public float m_reflectRadius;

	public GameObject m_reflectVisual;

	public override void UseBuff(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		ReflectBullets(p_playerRefrence, p_damageTargetMask, p_obstacleMask);
	}

	private void ReflectBullets(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask)
	{
		Collider2D[] colliders = Physics2D.OverlapCircleAll(p_playerRefrence.transform.position, m_reflectRadius, p_damageTargetMask);

		GameObject newObject = ObjectPooler.instance.NewObject(m_reflectVisual, p_playerRefrence.transform.position, Quaternion.identity);
		ParticleSystem newParticleSystem = newObject.GetComponent<ParticleSystem>();
		newParticleSystem.Play();

		foreach (Collider2D collider in colliders)
		{
			Vector3 normal = Vector3.Cross(p_playerRefrence.transform.position, collider.transform.position);

			Vector3 reflectDir = Vector3.Reflect(collider.transform.right, normal);

			float rotation = Mathf.Atan2(reflectDir.y, reflectDir.x) * Mathf.Rad2Deg;

			collider.transform.eulerAngles = new Vector3(0, 0, rotation);
		}
	}
}
