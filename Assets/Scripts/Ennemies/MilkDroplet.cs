using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MilkDroplet : MonoBehaviour {

    [SerializeField] private ParticleSystem explosionParticles;

    private Collider collider;

    [SerializeField] private float invulnerabilityTime;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip dropletClip;
    // Start is called before the first frame update
    void Start()
    {
        collider = GetComponent<Collider>();
        collider.enabled = false;
        
        Destroy(gameObject, 10);
    }

    // Update is called once per frame
    void Update()
    {
        invulnerabilityTime -= Time.deltaTime;

        if (invulnerabilityTime <= 0)
        {
            collider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        audioSource.pitch = Random.Range(0.8f, 0.9f);
        audioSource.clip = dropletClip;
        audioSource.Play();
        
        explosionParticles.Play();
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<TrailRenderer>());
        Destroy(gameObject, explosionParticles.main.duration);
    }
}
