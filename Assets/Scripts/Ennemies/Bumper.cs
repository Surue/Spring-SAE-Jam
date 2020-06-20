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
    [SerializeField] private float slowDownDistance = 20.0f;
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
    [SerializeField] private List<GameObject> smallParts;
    
    private Transform player;

    private float currentTimer = 0;

    //Movement
    private float movementAngle = 0;
    private float movementSpeed = 0;
    private Vector2 movementVector;
    
    private CarMovement carMovement;

    [Header("RayCast")]
    [SerializeField] private float distanceRayCast = 10f;
    [SerializeField] private LayerMask layerMaskRayCast;

    [Header("Audio")] 
    [SerializeField] private AudioSource audioSource;

    [Header("Colliders")] 
    [SerializeField] private Collider bumperCollider;
    [SerializeField] private Collider carCollider;
    
    enum State {
        PAUSE,
        IDLE, //Basic states to wait a few seconds before starting to roll again
        MOVE_TO_POSITION, //The bumper move to a "good" position to run on the player
        PREPARING_DASH, //The bumper stay still and load its dash. It's still able to slowly rotate
        DASH, //Dash over a given distance
        DYING //State to wait final destroy
    }

    private State state_ = State.IDLE;
    private State previousState_ = State.IDLE;
    
    // Start is called before the first frame update
    void Start() {
        player = FindObjectOfType<PlayerController>().transform;

        carMovement = GetComponent<CarMovement>();

        if (GameManager.Instance.CurrentState == GameManager.GameState.START)
        {
            previousState_ = state_;
            state_ = State.PAUSE;
        }
    }

    // Update is called once per frame
    void Update() {
        currentTimer += Time.deltaTime;

        movementVector = Vector2.zero;

        switch (state_) {
            case State.PAUSE:
                if (GameManager.Instance.CurrentState != GameManager.GameState.START)
                {
                    state_ = previousState_;
                }
                break;
            case State.IDLE:
                movementAngle = 0.0f;
                movementSpeed = 0.0f;

                //Transition to Move to position
                if (currentTimer > idleTimer)
                {
                    targetPosition = FindDashPosition();

                    currentTimer = 0.0f;
                    state_ = State.MOVE_TO_POSITION;
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

                if (distance < slowDownDistance)
                {
                    force = Vector2.zero;
                }
                
                Debug.DrawLine(transform.position + Vector3.up, transform.position + Vector3.up + new Vector3(force.x, 0, force.y));
                
                movementVector += force;
                
                EvaluateObstacleInFront();

                //Transition to Preparing dash
                if (distance < distanceToStop)
                {
                    currentTimer = 0.0f;
                    state_ = State.PREPARING_DASH;
                }
            }
                break;
            case State.PREPARING_DASH:
            {
                //TODO Slowly rotate towards player
                Vector3 dir = player.position - transform.position;
                movementAngle = Vector2.SignedAngle(new Vector2(dir.x, dir.z),
                    new Vector2(transform.forward.x, transform.forward.z));

                movementSpeed = 0.0f;

                //Transition to dash
                if (currentTimer > preparingDashTimer)
                {
                    currentTimer = 0.0f;
                    state_ = State.DASH;
                    carMovement.SetMaxSpeed(dashSpeed);
                }
            }
                break;
            case State.DASH:
                //TODO Dash a certain distance
                movementAngle = 0.0f;
                movementSpeed = dashSpeed;

                //Transition to idle
                if (Vector3.Distance(transform.position, targetPosition) > dashDistance)
                {
                    currentTimer = 0.0f;
                    state_ = State.IDLE;
                    
                    carMovement.SetMaxSpeed(basicSpeed);
                }
                break;
            case State.DYING:
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

        if (movementVector != Vector2.zero)
        {
            movementSpeed = movementVector.magnitude;
            movementAngle = Vector2.SignedAngle(new Vector2(movementVector.x, movementVector.y),
                new Vector2(transform.forward.x, transform.forward.z));
            
            
        }
        carMovement.Movement(movementAngle, movementSpeed);
    }

    /// <summary>
    /// Find the position from witch it will dash to the player
    /// </summary>
    /// <returns></returns>
    Vector3 FindDashPosition()
    {
        return player.transform.position + player.forward * distanceToTarget;
    }

    private void OnCollisionEnter(Collision other)
    {
        if (state_ == State.DASH)
        {
            targetPosition = (transform.position - targetPosition).normalized * dashDistance * 2;
        }

        //If hit player with the car collider => Die
        if (other.GetContact(0).thisCollider == carCollider && other.gameObject.CompareTag("Player") && state_ != State.DYING)
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

            foreach (var smallPart in smallParts)
            {
                smallPart.GetComponent<BoxCollider>().enabled = true;
                smallPart.AddComponent<Rigidbody>();
            }
            
            Destroy(carCollider);
            Destroy(bumperCollider);
            
            audioSource.Play();
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
    
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(targetPosition, 0.5f);
    } 
}
