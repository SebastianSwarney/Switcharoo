using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Player : Health
{
    [Header("Player Hit Properites")]
    public float m_invulnerableTime;
    public float m_movementControllLossTime;

    private PlayerController m_player;

    [SerializeField]
    private GameObject deathparticle;

    [SerializeField]
    private SpriteRenderer playerSprite;

    [SerializeField]
    int particleCount, numberOfRings, maxNumberOfRings;

    [SerializeField]
    float ringTimerDelay;

    public override void Start()
    {
        base.Start();
        m_player = GetComponent<PlayerController>();
        playerSprite = GetComponentInChildren<SpriteRenderer>();
    }

    public override void TakeDamage(float p_damage)
    {
        base.TakeDamage(p_damage);
        if (m_canTakeDamage)
        {
            m_player.m_playerHurt.Invoke();
        }
    }

    public override void Die()
    {
        //Add delay here and particle effect
        m_isDead = true;
        StartCoroutine(ChangeDeathColour());
        StartCoroutine(SpawnDeathParticles());
    }



    private IEnumerator ChangeDeathColour()
    {
        float duration = 4.0f;
        float totalTime = 0;

        while (totalTime <= duration)
        {
            totalTime += Time.deltaTime;

            Debug.Log("total time is" + totalTime);
            Color lerpedColor = Color.clear;
            lerpedColor = Color.Lerp(Color.white, Color.clear, totalTime*100f);
            playerSprite.color = lerpedColor;

            yield return null;
            Debug.Log("This Runs!!!");
        }
        //StartCoroutine(SpawnDeathParticles());
        //StartCoroutine(SpawnDeathParticles());
    }

    private IEnumerator SpawnDeathParticles()
    {

        while (numberOfRings < maxNumberOfRings)
        {
            float particleCountF = (float)particleCount;

            GameObject[] particlesArray = new GameObject[particleCount];

            for (int i = 0; i < particleCountF; i++)
            {
                float particlesAngleX = Mathf.Cos((i * (360 / particleCountF)) * Mathf.Deg2Rad);
                float particlesAngleY = Mathf.Sin((i * (360 / particleCountF)) * Mathf.Deg2Rad);

                float LerpRatio = ((1 / (particleCountF - 1)));

                float SpiralRadius = Mathf.Lerp(0, 0, LerpRatio * i);

                Vector3 SpiralRadiusCombined = new Vector3(particlesAngleX, particlesAngleY) * SpiralRadius;

                particlesArray[i]

                 = Instantiate(deathparticle, transform.position + SpiralRadiusCombined, Quaternion.Euler(0, 0, (i * (360 / particleCountF))));
                Debug.Log("particle Ring is " + numberOfRings);
            }
            yield return new WaitForSeconds(ringTimerDelay);
            numberOfRings++;

        }

    }
}
