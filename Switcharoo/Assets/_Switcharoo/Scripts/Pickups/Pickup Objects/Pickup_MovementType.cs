using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_MovementType : Pickup_Base
{
	public MovementType_Base[] m_movementTypes;

	private void Start()
	{
		m_amountOfItems = m_movementTypes.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetMovementTypePickup(m_movementTypes[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_movementTypes[m_currentItem].m_uiSprite;
	}
}
