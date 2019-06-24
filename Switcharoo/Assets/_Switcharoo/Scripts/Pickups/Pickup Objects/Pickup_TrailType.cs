using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_TrailType : Pickup_Base
{
	public TrailType_Base[] m_trailTypes;

	private void Start()
	{
		m_amountOfItems = m_trailTypes.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetTrailTypePickup(m_trailTypes[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_trailTypes[m_currentItem].m_uiSprite;
	}
}
