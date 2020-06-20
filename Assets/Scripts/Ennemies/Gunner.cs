using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gunner : MonoBehaviour
{
    [Header("Idle state")] 
    [SerializeField] private float idleTimer = 1.0f;

    [Header("Move to position state")] 
    [SerializeField] private float distanceToStop = 0.5f;
    [SerializeField] private float basicSpeed = 5.0f;
    private Vector3 targetPosition;

    [Header("Shooting")] 
    [SerializeField] private GameObject prefabProjectile;
    [SerializeField] private Transform shootingPos;

    [Header("Aim")] 
    [SerializeField] private float aimingTime = 3;
    [SerializeField] private Transform gun;
    
    [Header("Death")] 
    [SerializeField] private ParticleSystem explosionParticleSystem;
    [SerializeField] private float dyingTime = 3.0f;
    
    private Transform player;

    private float currentTimer = 0;

    //Movement
    private float movementAngle = 0;
    private float movementSpeed = 0;
    private Vector2 movementVector;
    
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
        IDLE,
        MOVE_TO_POSITION,
        AIM,
        SHOOT,
        DYING
    }

    private State state_ = State.IDLE;
    
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerController>().transform;

        material = meshRenderer.material;

        carMovement = GetComponent<CarMovement>();
    }

    // Update is called once per frame
    void Update()
    { 
        currentTimer += Time.deltaTime;

        movementVector = Vector2.zero;

        switch (state_) {
            case State.IDLE:
                movementAngle = 0.0f;
                movementSpeed = 0.0f;

                //Transition to Move to position
                if (currentTimer > idleTimer)
                {
                    //TODO Select position
                    targetPosition = FindShootPosition();

                    currentTimer = 0.0f;
                    state_ = State.MOVE_TO_POSITION;
                    material.color = Color.blue;
                }
                break;
            case State.MOVE_TO_POSITION:
            {
                //TODO Move towards position
                Vector3 dir = (targetPosition - transform.position).normalized;
                
                float distance = Vector2.Distance(
                    new Vector2(transform.position.x, transform.position.z),
                    new Vector2(targetPosition.x, targetPosition.z)
                    );

                Vector2 force = new Vector2(dir.x, dir.z) * basicSpeed;
                Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + new Vector3(force.x, 0, force.y));
                Debug.DrawLine(transform.position, targetPosition, Color.grey);
                movementVector += (Vector2)force;
                
                EvaluateObstacleInFront();

                //Transition to Preparing dash
                if (distance < distanceToStop)
                {
                    currentTimer = 0.0f;
                    state_ = State.AIM;
                    material.color = Color.yellow;
                }
            }
                break;
            case State.AIM:

                float angle = Vector3.SignedAngle(gun.forward, gun.position - player.position, Vector3.up);
                
                gun.forward = (player.position - gun.position).normalized;

                movementAngle = 0;
                movementSpeed = 0;

                if (currentTimer > aimingTime)
                {
                    currentTimer = 0;
                    state_ = State.SHOOT;
                    material.color = Color.red;
                }
                break;
            case State.SHOOT:
                GameObject instance = Instantiate(prefabProjectile, shootingPos.position, shootingPos.rotation);
                instance.GetComponent<Rigidbody>().velocity = instance.transform.forward * 50;
                // Destroy(instance, 5);

                state_ = State.IDLE;
                currentTimer = 0;
                material.color = Color.white;
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
    
    void EvaluateObstacleInFront()
    {
        Vector3 position = transform.position;
        Vector3 forward = transform.forward;

        BoxCastDrawer.DrawBoxCastBox(position + forward * 2, new Vector3(2, 1, 1), transform.rotation, forward, distanceRayCast, Color.blue);

        if (Physics.BoxCast(position + forward * 2, new Vector3(2, 1, 1), forward, out RaycastHit hitInfo, transform.rotation, distanceRayCast, layerMaskRayCast))
        {
            Vector3 targetDir = hitInfo.transform.position - position;
            Vector2 force;
            if (Vector3.Cross(targetDir, forward).z < 0)
            {
                force = new Vector2(-forward.z, forward.x).normalized * 5.0f;
            }
            else
            {
                force = new Vector2(forward.z, -forward.x).normalized * 5.0f;
            }
            
            movementVector += force;
            
            Debug.DrawLine(position + Vector3.up, position + Vector3.up + new Vector3(force.x, 0, force.y));
        }
    }

    private void FixedUpdate()
    {
        if (state_ == State.DYING) return;

        if (movementVector != Vector2.zero)
        {
            movementSpeed = movementVector.magnitude;
            movementAngle = Vector2.SignedAngle(new Vector2(movementVector.x, movementVector.y),
                new Vector2(transform.forward.x, transform.forward.z));
            
            
        }
        carMovement.Movement(movementAngle, movementSpeed);
    }

    Vector3 FindShootPosition()
    {
        return player.transform.position - player.forward * 10;
    }
    
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && state_ != State.DYING)
        {
            other.gameObject.GetComponent<PlayerController>().ScreenShake();
            
            //Kill bumper
            explosionParticleSystem.Play();

            state_ = State.DYING;

            currentTimer = 0.0f;
            
            Destroy(carMovement);
            Rigidbody body = GetComponent<Rigidbody>();
            body.velocity = (transform.position - other.GetContact(0).point).normalized * other.rigidbody.velocity.magnitude;
            body.constraints = RigidbodyConstraints.None;
            
            audioSource.Play();
        }
    }
}
