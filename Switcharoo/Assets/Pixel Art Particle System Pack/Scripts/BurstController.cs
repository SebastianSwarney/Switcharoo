using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BurstController : MonoBehaviour
{
    public List<ParticleSystem> particleSystems;

    void Start()
    {
        StartCoroutine(EmitBursts());
    }

    IEnumerator EmitBursts()
    {
        while (true)
        {
            yield return new WaitForSeconds(2.0f);

            foreach (ParticleSystem ps in particleSystems)
            {
                ps.Emit(1);
                ps.Play();
            }

            if (particleSystems.Count <= 0)
            {
                break;
            }
        }

        yield return null;
    }
}
