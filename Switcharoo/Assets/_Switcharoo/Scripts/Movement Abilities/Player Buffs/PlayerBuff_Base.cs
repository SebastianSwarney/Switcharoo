﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerBuff_Base : ScriptableObject
{
	public abstract void UseBuff(PlayerController p_playerRefrence, LayerMask p_damageTargetMask, LayerMask p_obstacleMask);
}