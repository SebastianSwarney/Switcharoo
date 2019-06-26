using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class FireData : ScriptableObject
{
	public int m_minFireHitsPerEffect;
	public int m_maxFireHitsPerEffect;

	public float m_maxFireEffectInterval;
	public float m_minFireEffectInterval;

	public float m_fireDamageAmount;
}
