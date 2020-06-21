using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObstacle : Obstacle
{
    [SerializeField] private GameObject model;
    [SerializeField] private ParticleSystem explosionParticle;
    [SerializeField] private float explosionDamage = 10.0f;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip impactClip;

    protected override void Destruct(Collision collision)
    {
        obstacleSpawner.Remove(gameObject);

        explosionParticle.Play();
        Destroy(gameObject, explosionParticle.main.duration);
        Destroy(GetComponent<Rigidbody>());
        Destroy(model);
        Destroy(GetComponent<BoxCollider>());

        audioSource.pitch = Random.Range(0.9f, 1.0f);
        audioSource.clip = impactClip;
        audioSource.Play();
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerController>().Hit(explosionDamage);
        }
    }
}
