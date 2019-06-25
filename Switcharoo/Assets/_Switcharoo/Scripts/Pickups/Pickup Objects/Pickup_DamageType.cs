using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_DamageType : Pickup_Base
{
	public DamageType_Base[] m_damageTypes;

	private void Start()
	{
		m_amountOfItems = m_damageTypes.Length;
		RandomItemSelection();
	}

	public override void SetPickup(PlayerController p_playerRefrence)
	{
		p_playerRefrence.SetDamageTypePickup(m_damageTypes[m_currentItem]);
	}

	public override void ChangeItem()
	{
		base.ChangeItem();
		m_itemIconRenderer.sprite = m_damageTypes[m_currentItem].m_uiSprite;
	}
}
