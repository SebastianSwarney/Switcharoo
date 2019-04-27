using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Bullet Types")]
public class BulletType_Base : ScriptableObject
{
	public GameObject m_bulletObject;

	public Sprite[] m_chargeSprites;
}
