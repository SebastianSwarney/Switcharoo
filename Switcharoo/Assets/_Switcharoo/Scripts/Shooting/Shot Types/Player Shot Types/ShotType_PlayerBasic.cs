using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/Player Shot Types/Basic Shot")]
public class ShotType_PlayerBasic : ShotType_Base
{
    public GameObject m_bulletPrefab;

	public override void Shoot(Transform p_bulletOrgin)
	{
        GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletPrefab, p_bulletOrgin);

        newBullet.transform.position = p_bulletOrgin.position;
	    newBullet.transform.rotation = p_bulletOrgin.rotation;
	}
}
