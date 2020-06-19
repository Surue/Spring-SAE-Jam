using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Bumper : MonoBehaviour {

    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Move to position state")] 
    [SerializeField] private float distanceToStop = 0.5f;
    [SerializeField] private float basicSpeed = 5.0f;
    private Vector3 targetPosition;
    
    [Header("Preparing dash state")] 
    [SerializeField] private float preparingDashTimer = 5.0f;
    [SerializeField] private float distanceToTarget = 5.0f;

    [Header("Dash")] 
    [SerializeField] private float dashSpeed = 5.0f;
    [SerializeField] private float dashDistance = 7.5f;

    [Header("Death")] 
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    
    private Transform player;

    private float currentTimer = 0;

    //Movement
    private float movementAngle = 0;
    private float movementSpeed = 0;
    
    private CarMovement carMovement;

    [Header("RayCast")]
    [SerializeField] private float distanceRayCast = 10f;
    [SerializeField] private Vector3 offsetRayCast;
    [SerializeField][Range(0.0f, 10.0f)] private float widenessRayCast = 1;
    [SerializeField][Range(0, 10)] private int iterationRayCast = 2;
    [SerializeField] private LayerMask layerMaskRayCast;
    
    [Header("Rendering")]
    [SerializeField] private MeshRenderer meshRenderer;
    private Material material;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;
    
    enum State {
        IDLE, //Basic states to wait a few seconds before starting to roll again
        MOVE_TO_POSITION, //The bumper move to a "good" position to run on the player
        PREPARING_DASH, //The bumper stay still and load its dash. It's still able to slowly rotate
        DASH, //Dash over a given distance
        DYING //State to wait final destroy
    }

    private State state_ = State.MOVE_TO_POSITION;
    
    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<PlayerController>().transform;

        material = meshRenderer.material;

        carMovement = GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update() {
        currentTimer += Time.deltaTime;

        switch (state_) {
            case State.IDLE:
                movementAngle = 0;
                movementSpeed = 0;

                //Transition to Move to position
                if (currentTimer > idleTimer)
                {
                    //TODO Select position
                    targetPosition = player.transform.position + player.forward * distanceToTarget;

                    currentTimer = 0;
                    state_ = State.MOVE_TO_POSITION;
                    material.color = Color.blue;
                }
                break;
            case State.MOVE_TO_POSITION:
            {
                //TODO Move towards position
                Vector3 dir = targetPosition - transform.position;
                movementAngle = Vector2.SignedAngle(new Vector2(dir.x, dir.z),
                    new Vector2(transform.forward.x, transform.forward.z));

                float distance = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(targetPosition.x, targetPosition.z)
                    );

                movementSpeed =  Mathf.Pow(Mathf.Clamp01(distance / 5.0f), 2) * basicSpeed;
                
                EvaluateObstacleInFront();

                //Transition to Preparing dash
                if (distance < distanceToStop)
                {
                    currentTimer = 0;
                    state_ = State.PREPARING_DASH;
                    material.color = Color.yellow;
                }
            }
                break;
            case State.PREPARING_DASH:
            {
                //TODO Slowly rotate towards player
                Vector3 dir = player.position - transform.position;
                movementAngle = Vector2.SignedAngle(new Vector2(dir.x, dir.z),
                    new Vector2(transform.forward.x, transform.forward.z));

                movementSpeed = 0;

                //Transition to dash
                if (currentTimer > preparingDashTimer)
                {
                    currentTimer = 0;
                    material.color = Color.red;
                    state_ = State.DASH;
                    carMovement.SetMaxSpeed(dashSpeed);
                }
            }
                break;
            case State.DASH:
                //TODO Dash a certain distance
                movementAngle = 0;
                movementSpeed = dashSpeed;

                //Transition to idle
                if (Vector3.Distance(transform.position, targetPosition) > dashDistance)
                {
                    currentTimer = 0;
                    state_ = State.IDLE;
                    material.color = Color.white;
                    
                    carMovement.SetMaxSpeed(basicSpeed);
                }
                break;
            case State.DYING:
                material.color = Color.Lerp(Color.white, Color.clear, currentTimer / dyingTime);
                
                if (currentTimer > dyingTime)
                {
                    Destroy(gameObject);
                }
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void FixedUpdate()
    {
        if (state_ == State.DYING) return;
        
        carMovement.Movement(movementAngle, movementSpeed);
    }

    /// <summary>
    /// Find the position from witch it will dash to the player
    /// </summary>
    /// <returns></returns>
    Vector3 FindDashPosition()
    {
        return new Vector3();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision");
        //TODO Defines tag or layer
        if (state_ == State.DASH)
        {
            targetPosition = (transform.position - targetPosition).normalized * dashDistance * 2;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<PlayerController>().ScreenShake();
            
            //Kill bumper
            explosionParticleSystem.Play();

            state_ = State.DYING;

            currentTimer = 0;
            
            Destroy(carMovement);
            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity = (transform.position - other.GetContact(0).point).normalized * other.rigidbody.velocity.magnitude;
            body.constraints = RigidbodyConstraints.None;
            
            audioSource.Play();
        }
    }

    void EvaluateObstacleInFront()
    {
        Vector3 posRayCast = transform.position + offsetRayCast;
        Vector3 front = posRayCast + transform.forward * distanceRayCast;
        
        Vector3 dir;
        
        float maxAngle = 90;
        
        for (int i = -iterationRayCast; i <= iterationRayCast; i++)
        {
            dir = (front + transform.right * i * widenessRayCast - posRayCast).normalized;

            if ( Physics.Raycast(posRayCast + transform.right * i * 0.25f, dir, out var hitInfo, distanceRayCast, layerMaskRayCast)) {
                Debug.DrawLine(posRayCast + transform.right * i * 0.25f, hitInfo.point, Color.red);
                
                Vector3 targetDir = hitInfo.point - transform.position;
                
                float angle = Vector2.SignedAngle(
                    new Vector2(transform.forward.x, transform.forward.z),
                    new Vector2(targetDir.x, targetDir.z));

                movementAngle += angle * Mathf.Clamp01(1 -  Mathf.Abs((angle / maxAngle))) * 5;
            }
            else
            {
                Debug.DrawLine(posRayCast + transform.right * i * 0.25f, posRayCast + dir * distanceRayCast, Color.white);
            }
        }
    }
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
    } 
}
