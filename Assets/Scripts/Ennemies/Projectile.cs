using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Projectile : MonoBehaviour {
    [SerializeField] private float maxLifeTime = 5;
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem explosionParticle;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip impactClip;
    
    private float lifeTime = 0;
    
    void Update()
    {
        lifeTime += Time.deltaTime;

        if (lifeTime >= maxLifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        //Check to don't touch self
        if (lifeTime > 0.1f)
        {
            explosionParticle.Play();
            Destroy(gameObject, explosionParticle.main.duration);
            Destroy(GetComponent<Rigidbody>());
            Destroy(model);
            Destroy(GetComponent<SphereCollider>());

            audioSource.pitch = Random.Range(0.9f, 1.0f);
            audioSource.clip = impactClip;
            audioSource.Play();
        }

    }
}
