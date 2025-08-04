using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleSpawner : MonoBehaviour
{
    public static ParticleSpawner instance;
    public GameObject succubusDisappearParticle;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SpawnParticle(GameObject particle, Vector3 position)
    {
        GameObject particleInstance = Instantiate(particle, position, Quaternion.identity);
        Destroy(particleInstance, particle.GetComponent<ParticleSystem>().main.duration);
    }
}