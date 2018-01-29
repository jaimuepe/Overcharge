using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSystemPoolManager : MonoBehaviour
{
    public int startSize;
    public ParticleSystem particleSystemPrefab;

    List<ParticleSystem> particleSystems = new List<ParticleSystem>();

    void Start()
    {
        for (int i = 0; i < startSize; i++)
        {
            ParticleSystem ps = Instantiate(particleSystemPrefab);
            ps.transform.parent = transform;
            particleSystems.Add(ps);
            ps.gameObject.SetActive(false);
        }
    }

    public ParticleSystem Request()
    {
        for (int i = 0; i < startSize; i++)
        {
            if (!particleSystems[i].isPlaying)
            {
                return particleSystems[i];
            }
        }

        ParticleSystem ps = Instantiate(particleSystemPrefab);
        ps.transform.parent = transform;
        particleSystems.Add(ps);
        ps.gameObject.SetActive(false);

        return ps;
    }
}
