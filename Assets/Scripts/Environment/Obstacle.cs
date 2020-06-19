using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Rigidbody rigidbody;
    [SerializeField] private float forceSpeed;
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
            rigidbody.AddForce((transform.position - other.transform.position + Vector3.up) * forceSpeed * other.relativeVelocity.magnitude);
            other.gameObject.GetComponent<CarMovement>().BumpObstacle();
        }
    }
}
