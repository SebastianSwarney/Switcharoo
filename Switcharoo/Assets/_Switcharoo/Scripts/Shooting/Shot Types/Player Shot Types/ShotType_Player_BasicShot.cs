using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/Player Shot Types/Basic Shot")]
public class ShotType_Player_BasicShot : ShotType_Player
{
	public override void Shoot(Transform p_bulletOrigin)
	{
		GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletPrefab, p_bulletOrigin);

		newBullet.transform.position = p_bulletOrigin.position;
		newBullet.transform.rotation = p_bulletOrigin.rotation;
	}
}
