using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_ShotPattern : Pickup_Base
{
	public ShotPattern_Base[] m_shotPatterns;

	private void Start()
	{
		m_amountOfItems = m_shotPatterns.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetShotPatternPickup(m_shotPatterns[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_shotPatterns[m_currentItem].m_uiSprite;
	}
}
