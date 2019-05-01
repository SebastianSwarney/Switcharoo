using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Shot Types/AI/Single Shot")]
public class ShotType_Player_Single : ShotType_Base
{
    public override void Shoot(Transform p_bulletOrigin)
    {
        ShootBasicShot(p_bulletOrigin, m_bulletType.m_chargeSprites[0]);
    }

    private void ShootBasicShot(Transform p_bulletOrigin, Sprite p_bulletSprite)
    {
        GameObject newBullet = ObjectPooler.instance.NewObject(m_bulletType.gameObject, p_bulletOrigin);
        newBullet.GetComponent<Bullet_Base>().InitializeParameters(p_bulletSprite, m_bulletSpeed);
    }

}
