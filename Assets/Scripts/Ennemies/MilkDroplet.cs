using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MilkDroplet : MonoBehaviour {

    [SerializeField] private ParticleSystem explosionParticles;

    private Collider collider;

    [SerializeField] private float invulnerabilityTime;
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
        explosionParticles.Play();
        Destroy(GetComponent<MeshRenderer>());
        Destroy(GetComponent<TrailRenderer>());
        Destroy(gameObject, explosionParticles.main.duration);
        
        Debug.Log("milk pop");
    }
}
