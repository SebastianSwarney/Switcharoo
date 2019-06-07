using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmCanon_AnimationController : MonoBehaviour
{
    public GameObject m_bulletCasing;
    public float m_bulletEjectionForce;
    [Range(0, 100)]
    public float m_randomCasingAngularForce;

    public Transform m_lookAt;
    private void Update()
    {
        float angle = Mathf.Atan2(m_lookAt.position.y - transform.position.y, m_lookAt.transform.position.x - transform.position.x) * Mathf.Rad2Deg;
        transform.eulerAngles = new Vector3(0f, 0f, angle);
    }
    public void EjectBullet()
    {
        float angularForce = Random.Range(0, m_randomCasingAngularForce);
        GameObject casing = ObjectPooler.instance.NewObject(m_bulletCasing, transform);
        Rigidbody2D rb2D = casing.GetComponent<Rigidbody2D>();
        rb2D.angularVelocity = angularForce;
        rb2D.velocity = -transform.right * m_bulletEjectionForce;

    }
}
