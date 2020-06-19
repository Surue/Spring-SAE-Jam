using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

[RequireComponent(typeof(Rigidbody))]
public class CarMovement : MonoBehaviour
{
    private Rigidbody rigidbody;
    float currentSpeed = 0.0f;

    public float CurrentSpeed {
        get => currentSpeed;
    }

    [SerializeField] private float slowdown = 1.0f;
    [SerializeField] private float rotationSpeed = 1.0f;
    [SerializeField] private float linearSpeed = 1.0f;
    [SerializeField] private float maxSpeed = 1.0f;
    [SerializeField] private float minSpeed = 0.005f;

    public float MaxSpeed => maxSpeed;


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        currentSpeed = rigidbody.velocity.magnitude;
    }

    //To call in fixedUpdate
    public void Movement(float rotationInput, float speedInput)
    {
        if (Math.Abs(speedInput) < minSpeed)
        {
            if (currentSpeed > minSpeed)
            {
                currentSpeed -= slowdown * Time.fixedDeltaTime;
            }
            else if(currentSpeed < minSpeed)
            {
                currentSpeed += slowdown * Time.fixedDeltaTime;
            } else
            {
                currentSpeed = 0.0f;
            }
        } else if (currentSpeed > maxSpeed)
        {
            currentSpeed = maxSpeed;
        }
        transform.eulerAngles += new Vector3(0, rotationInput, 0) * rotationSpeed * Time.fixedDeltaTime;
        currentSpeed += speedInput * linearSpeed * Time.fixedDeltaTime;
        rigidbody.velocity = transform.forward * currentSpeed + Vector3.up*rigidbody.velocity.y;
    }
    
    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxSpeed = newMaxSpeed;
    }

    public void BumpObstacle()
    {
        currentSpeed /= 2;
    }
}
