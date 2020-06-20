using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float forceSpeed;
    [SerializeField] private float heightSpeed;
    [SerializeField] private float screenAmplitudeGain = 10f;
    [SerializeField] private float screenFrequencyGain = 10f;
    [SerializeField] private float carAmplitudeGain = 5f;
    [SerializeField] private float carFrequencyGain = 50f;
    [SerializeField] private float screenshakeDuration = 1f;
    [SerializeField] private float carshakeDuration = 1f;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.GetComponent<CarMovement>())
        {
            rigidbody.AddForce((transform.position - other.transform.position) * forceSpeed * other.relativeVelocity.magnitude + Vector3.up * heightSpeed );
            //other.gameObject.GetComponent<CarMovement>().BumpObstacle();
            if (other.gameObject.CompareTag("Player"))
            {
                other.gameObject.GetComponent<PlayerController>().ScreenShake(screenAmplitudeGain, screenFrequencyGain, screenshakeDuration);
                other.gameObject.GetComponent<PlayerController>().CarShake(carAmplitudeGain, carFrequencyGain, carshakeDuration);
            }
        }
    }
}
