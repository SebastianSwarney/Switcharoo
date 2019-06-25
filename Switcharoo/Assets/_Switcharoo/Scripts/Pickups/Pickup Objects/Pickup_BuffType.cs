using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_BuffType : Pickup_Base
{
	public PlayerBuff_Base[] m_buffTypes;

	private void Start()
	{
		m_amountOfItems = m_buffTypes.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetBuffPickup(m_buffTypes[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_buffTypes[m_currentItem].m_uiSprite;
	}
}
