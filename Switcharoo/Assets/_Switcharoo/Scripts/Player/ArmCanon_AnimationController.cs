using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class ArmCanon_AnimationController : MonoBehaviour
{
    public GameObject m_bulletCasing;
    public float m_bulletEjectionForce;
    [Range(0, 100)]
    public float m_randomCasingAngularForce;

	private PlayerController m_player;

	public Transform m_casingOrgin;

	private ShootController m_shootController;

	private Animator m_animator;

	private void Start()
	{
		m_player = GetComponentInParent<PlayerController>();
		m_shootController = GetComponentInParent<ShootController>();

		m_animator = GetComponent<Animator>();
	}

	private void Update()
    {
		transform.rotation = m_player.m_crosshair.rotation;
    }

	public void ShootBullet()
	{
		m_animator.speed = m_shootController.m_currentWeaponComposition.m_shotPattern.m_fireRate;
		m_animator.SetTrigger("Fire");
	}

    public void EjectBullet()
    {
		//float angularForce = Random.Range(0, m_randomCasingAngularForce);

		float bulletRotation = m_casingOrgin.rotation.z + Random.Range(m_randomCasingAngularForce, -m_randomCasingAngularForce);

		Quaternion bulletRotationQuaternion = Quaternion.Euler(0, 0, bulletRotation);

		GameObject casing = ObjectPooler.instance.NewObject(m_bulletCasing, m_casingOrgin);
        Rigidbody2D rb2D = casing.GetComponent<Rigidbody2D>();

		casing.transform.rotation = m_casingOrgin.rotation * bulletRotationQuaternion;

		rb2D.AddForce(casing.transform.right * m_bulletEjectionForce, ForceMode2D.Impulse);
	}
}
