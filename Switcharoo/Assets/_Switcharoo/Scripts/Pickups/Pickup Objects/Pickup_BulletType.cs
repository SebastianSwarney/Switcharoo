using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_BulletType : Pickup_Base
{
	public Bullet_Base[] m_bulletTypes;

	private void Start()
	{
		m_amountOfItems = m_bulletTypes.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetBulletTypePickup(m_bulletTypes[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_bulletTypes[m_currentItem].m_uiSprite;
	}
}
