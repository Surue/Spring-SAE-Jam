using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    protected ObstaclesSpawner obstacleSpawner;
    private Rigidbody rigidbody;
    [SerializeField] private float forceSpeed;
    [SerializeField] private float heightSpeed;
    [SerializeField] private float lifeTimeAfterExplode = 2f;

    [Header("Screen Shake")]
    [SerializeField] private float screenAmplitudeGain = 10f;
    [SerializeField] private float screenFrequencyGain = 10f;
    [SerializeField] private float screenshakeDuration = 1f;
    [Header("Car Shake")]
    [SerializeField] private float carAmplitudeGain = 5f;
    [SerializeField] private float carFrequencyGain = 50f;
    [SerializeField] private float carshakeDuration = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        obstacleSpawner = FindObjectOfType<ObstaclesSpawner>();
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<CarMovement>())
        {
            Destruct(other);
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().ScreenShake(screenAmplitudeGain, screenFrequencyGain, screenshakeDuration);
                other.gameObject.GetComponent<PlayerController>().CarShake(carAmplitudeGain, carFrequencyGain, carshakeDuration);
            }
        }
    }

    protected virtual void Destruct(Collision collision)
    {
        if (obstacleSpawner != null)
        {
            obstacleSpawner.Remove(transform.parent.gameObject);
        }

        rigidbody.AddForce((transform.position - collision.transform.position) * forceSpeed * collision.relativeVelocity.magnitude + Vector3.up * heightSpeed);
        Destroy(transform.parent.gameObject, lifeTimeAfterExplode);
    }
}
